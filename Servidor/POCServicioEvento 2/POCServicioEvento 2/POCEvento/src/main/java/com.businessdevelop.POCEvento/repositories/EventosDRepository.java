package com.businessdevelop.POCEvento.repositories;

import com.businessdevelop.POCEvento.model.EventoDeportivo;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;

import java.util.List;

public interface EventosDRepository extends JpaRepository<EventoDeportivo, String> {

    // Consulta donde se muestra informacion de Maestro con detalle
    @Query("SELECT e FROM EventoDeportivo e LEFT JOIN FETCH e.equipos")
    List<EventoDeportivo> listarEventosConEquipos();

    // Filtro por ciudad y tipo deporte (JPQL)
    @Query("""
           SELECT e 
           FROM EventoDeportivo e 
           WHERE (:ciudad IS NULL OR e.ciudad = :ciudad)
             AND (:tipoDeporte IS NULL OR e.tipoDeporte = :tipoDeporte)
           """)
    List<EventoDeportivo> filtrarEventos(String ciudad, String tipoDeporte);
}
