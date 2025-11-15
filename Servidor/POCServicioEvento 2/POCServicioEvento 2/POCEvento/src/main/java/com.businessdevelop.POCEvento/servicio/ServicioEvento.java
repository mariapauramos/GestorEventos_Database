package com.businessdevelop.POCEvento.servicio;
import com.businessdevelop.POCEvento.dto.EventoConSedeDTO;
import com.businessdevelop.POCEvento.dto.SedeResumenDTO;
import com.businessdevelop.POCEvento.model.Evento;
import com.businessdevelop.POCEvento.model.Equipo;
import com.businessdevelop.POCEvento.model.EventoDeportivo;
import com.businessdevelop.POCEvento.repositories.EquiposRepository;
import com.businessdevelop.POCEvento.repositories.EventosDRepository;
import org.springframework.beans.factory.annotation.Value;
import jakarta.transaction.Transactional;
import org.springframework.stereotype.Service;
import org.springframework.web.client.HttpClientErrorException;
import org.springframework.web.client.RestTemplate;

import java.util.ArrayList;
import java.util.List;
import java.util.Optional;
import java.util.stream.Collectors;

@Service
public class ServicioEvento {

    private final EventosDRepository repositorio;
    private final EquiposRepository equiposRepository;
    private final RestTemplate restTemplate;

    @Value("${sede.service.url}")
    private String sedeServiceUrl;

    public ServicioEvento(EventosDRepository repositorio,
                          EquiposRepository equiposRepository,
                          RestTemplate restTemplate) {
        this.repositorio = repositorio;
        this.equiposRepository = equiposRepository;
        this.restTemplate = restTemplate;
    }

    //CRUD de gestion de eventos

    //1. Crear evento
    @Transactional
    public EventoDeportivo crearEvento(EventoDeportivo evento) {

        if (repositorio.existsById(evento.getIdEvento())) {
            throw new IllegalArgumentException(
                    "Ya existe un evento con id: " + evento.getIdEvento());
        }
        return repositorio.save(evento);
    }

    //Buscar evento
    public Optional<EventoDeportivo> buscarEvento(String idEvento) {

        return repositorio.findById(idEvento);
    }

    //Listar todos los eventos
    public List<EventoDeportivo> listarEventos() {

        return repositorio.findAll();
    }

    //Listar solo los eventos deportivos con equipos
    public List<EventoDeportivo> listarEventosConEquipos() {

        return repositorio.listarEventosConEquipos();
    }

    //Listar eventos deportivos por 2 parametros los cuales son: ciudad y tipo de deporte
    public List<EventoDeportivo> listarFiltroED(String ciudad, String tipoDeporte) {
        return repositorio.filtrarEventos(ciudad, tipoDeporte);
    }

    //Actualizar evento deportivo con busqueda previa
    @Transactional
    public boolean actualizarEvento(String idEvento, EventoDeportivo actualizado) {

        Optional<EventoDeportivo> opt = repositorio.findById(idEvento);
        if (opt.isEmpty()) {
            return false;
        }
        EventoDeportivo existente = opt.get();

        existente.setNombre(actualizado.getNombre());
        existente.setCiudad(actualizado.getCiudad());
        existente.setAsistentes(actualizado.getAsistentes());
        existente.setFecha(actualizado.getFecha());
        existente.setTipoDeporte(actualizado.getTipoDeporte());

        if (actualizado.getEquipos() != null && !actualizado.getEquipos().isEmpty()) {
            existente.setEquipos(actualizado.getEquipos());
        }

        repositorio.save(existente);
        return true;
    }

    //Eliminar evento
    @Transactional
    public boolean eliminarEvento(String idEvento) {
        EventoDeportivo existente = repositorio.findById(idEvento).orElse(null);
        if (existente == null) {
            return false;
        }

        // Desasociar todos los equipos asociados a este evento
        List<Equipo> equiposAsociados = equiposRepository.findByEventoDeportivo(existente);
        for (Equipo equipo : equiposAsociados) {
            equipo.setEventoDeportivo(null);
            equiposRepository.save(equipo);
        }


        repositorio.deleteById(idEvento);
        return true;
    }

    public EventoConSedeDTO buscarEventoConSede(String idEvento) {
        EventoDeportivo evento = repositorio.findById(idEvento)
                .orElseThrow(() -> new RuntimeException("Evento no encontrado con id: " + idEvento));

        EventoConSedeDTO dto = new EventoConSedeDTO();
        dto.setIdEvento(evento.getIdEvento());
        dto.setNombre(evento.getNombre());
        dto.setCiudad(evento.getCiudad());
        dto.setAsistentes(evento.getAsistentes());
        dto.setFecha(evento.getFecha());
        dto.setTipoDeporte(evento.getTipoDeporte());
        dto.setEquipos(evento.getEquipos()); // ya tienes la relación con equipos

        try {
            // Llamada al microservicio de sedes
            String url = sedeServiceUrl + "/sedes/porEvento/" + idEvento;
            SedeResumenDTO sede = restTemplate.getForObject(url, SedeResumenDTO.class);
            dto.setSede(sede);
        } catch (HttpClientErrorException.NotFound e) {
            // No hay sede asociada → sede = null
            dto.setSede(null);
        } catch (Exception e) {
            // Error inesperado llamando al otro microservicio
            System.out.println("Error consultando sede en microservicio C: " + e.getMessage());
            dto.setSede(null);
        }

        return dto;
    }
}
