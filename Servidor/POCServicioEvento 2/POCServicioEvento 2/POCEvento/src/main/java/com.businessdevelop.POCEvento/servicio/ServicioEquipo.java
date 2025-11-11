
package com.businessdevelop.POCEvento.servicio;

import com.businessdevelop.POCEvento.model.Equipo;
import com.businessdevelop.POCEvento.model.Evento;
import com.businessdevelop.POCEvento.model.EventoDeportivo;
import com.businessdevelop.POCEvento.repositories.EquiposRepository;
import com.businessdevelop.POCEvento.repositories.EventosDRepository;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.ArrayList;
import java.util.List;
import java.util.Optional;

@Service
public class ServicioEquipo {

    private final EquiposRepository repositorio;
    private final EventosDRepository repositorioEvento;

    public ServicioEquipo(EquiposRepository repositorio, EventosDRepository repositorioEvento) {
        this.repositorio = repositorio;
        this.repositorioEvento = repositorioEvento;
    }

    //CRUD de gestion de equipos

    //1. Crear equipo
    @Transactional
    public Equipo crearEquipo(Equipo equipo) {

        if (repositorio.existsById(equipo.getIdEquipo())) {
            throw new IllegalArgumentException(
                    "Ya existe un equipo con id: " + equipo.getIdEquipo()
            );
        }
        if (equipo.getEventoDeportivo() != null &&
                equipo.getEventoDeportivo().getIdEvento() != null) {

            String idEvento = equipo.getEventoDeportivo().getIdEvento();
            EventoDeportivo eventoCompleto = repositorioEvento.findById(idEvento)
                    .orElseThrow(() -> new RuntimeException("Evento no encontrado"));

            equipo.setEventoDeportivo(eventoCompleto);
        }

        return repositorio.save(equipo);
    }

    //Buscar equipo
    public Optional<Equipo> buscarEquipo(Integer idEquipo) {

        return repositorio.findById(idEquipo);
    }

    //Listar todos los equipos
    public List<Equipo> listarEquipos() {

        return repositorio.findAll();
    }

    //Listar equipos por 1 parametro el cual es: numero de jugadores
    public List<Equipo> listarPorNumeroJugadores(int numeroJugadores) {
        return repositorio.findByNumeroJugadores(numeroJugadores);
    }


    //Actualizar equipo
    @Transactional
    public boolean actualizarEquipo(Integer idEquipo, Equipo actualizado) {
        Equipo existente = repositorio.findById(idEquipo).orElse(null);
        if (existente == null) {
            return false;
        }

        existente.setNombre(actualizado.getNombre());
        existente.setCiudadOrigen(actualizado.getCiudadOrigen());
        existente.setNumeroJugadores(actualizado.getNumeroJugadores());
        existente.setPuntaje(actualizado.getPuntaje());

        // 🔍 Depuración
        System.out.println("=== ACTUALIZACIÓN DE EQUIPO ===");
        System.out.println("ID Equipo: " + idEquipo);
        System.out.println("Evento recibido: " + actualizado.getEventoDeportivo());
        if (actualizado.getEventoDeportivo() != null) {
            System.out.println("ID Evento recibido: " + actualizado.getEventoDeportivo().getIdEvento());
        }

        if (actualizado.getEventoDeportivo() != null &&
                actualizado.getEventoDeportivo().getIdEvento() != null) {

            String idEvento = actualizado.getEventoDeportivo().getIdEvento().trim();
            EventoDeportivo eventoCompleto = repositorioEvento.findById(idEvento)
                    .orElseThrow(() -> new RuntimeException("Evento no encontrado con id: " + idEvento));

            existente.setEventoDeportivo(eventoCompleto);
            System.out.println("Asociado evento " + idEvento + " al equipo " + idEquipo);

        } else {
            existente.setEventoDeportivo(null);
            System.out.println("⚠️ Se desasoció el evento del equipo " + idEquipo);
        }

        repositorio.save(existente);
        System.out.println("=== EQUIPO ACTUALIZADO ===");
        return true;
    }


    //Eliminar equipoo
    public boolean eliminarEquipo(Integer idEquipo) {
        Optional<Equipo> optionalEquipo = repositorio.findById(idEquipo);

        if (optionalEquipo.isPresent()) {
            Equipo equipo = optionalEquipo.get();

            // 👇 Desasociar el evento antes de eliminar
            if (equipo.getEventoDeportivo() != null) {
                equipo.getEventoDeportivo().getEquipos().remove(equipo); // limpiar lado inverso
                equipo.setEventoDeportivo(null);
            }

            repositorio.delete(equipo);
            return true;
        } else {
            return false;
        }

    }
}
