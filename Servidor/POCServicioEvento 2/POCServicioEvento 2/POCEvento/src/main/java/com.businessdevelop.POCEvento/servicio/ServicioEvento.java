package com.businessdevelop.POCEvento.servicio;
import com.businessdevelop.POCEvento.model.Evento;
import com.businessdevelop.POCEvento.model.Equipo;
import com.businessdevelop.POCEvento.model.EventoDeportivo;
import com.businessdevelop.POCEvento.repositories.EquiposRepository;
import com.businessdevelop.POCEvento.repositories.EventosDRepository;
import jakarta.transaction.Transactional;
import org.springframework.stereotype.Service;

import java.util.ArrayList;
import java.util.List;
import java.util.Optional;
import java.util.stream.Collectors;

@Service
public class ServicioEvento {

    private final EventosDRepository repositorio;
    private final EquiposRepository equiposRepository;

    public ServicioEvento(EventosDRepository repositorio, EquiposRepository equiposRepository) {
        this.repositorio = repositorio;
        this.equiposRepository = equiposRepository;
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
}
