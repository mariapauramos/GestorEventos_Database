package com.businessdevelop.POCEvento.dto;

import com.businessdevelop.POCEvento.model.Equipo;
import lombok.Data;

import java.time.LocalDate;
import java.util.List;

@Data
public class EventoConSedeDTO {

    private String idEvento;
    private String nombre;
    private String ciudad;
    private int asistentes;
    private LocalDate fecha;
    private String tipoDeporte;

    // Puedes incluir equipos si quieres
    private List<Equipo> equipos;

    // Sede asociada (puede ser null si no existe)
    private SedeResumenDTO sede;
}