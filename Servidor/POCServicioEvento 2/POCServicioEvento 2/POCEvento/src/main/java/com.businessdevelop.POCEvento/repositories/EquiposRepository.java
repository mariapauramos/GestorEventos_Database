package com.businessdevelop.POCEvento.repositories;
import com.businessdevelop.POCEvento.model.Equipo;
import com.businessdevelop.POCEvento.model.EventoDeportivo;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;

import java.util.List;

public interface EquiposRepository extends JpaRepository<Equipo, Integer> {

    // FILTRO: Buscar por ciudad
    @Query("SELECT eq FROM Equipo eq WHERE eq.ciudadOrigen = :ciudad")
    List<Equipo> buscarPorCiudad(String ciudad);

    // FILTRO: Buscar por número de jugadores
    List<Equipo> findByNumeroJugadores(int numeroJugadores);

    // Metodo de buscar equipos por evento
    List<Equipo> findByEventoDeportivo(EventoDeportivo eventoDeportivo);

}

