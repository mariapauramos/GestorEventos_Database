package com.businessdevelop.POCEvento.dto;

import lombok.Data;

import java.time.LocalDateTime;

@Data
public class SedeResumenDTO {
    private Integer idSede;
    private String nombre;
    private String direccion;
    private int capacidad;
    private LocalDateTime fechaCreacion;
    private boolean cubierta;

    private double costoMantenimiento;   // AGREGAR
    private String idEventoAsociado;
    private String nombreEvento;
}
