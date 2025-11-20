package com.businessdevelop.POCEvento.dto;

import lombok.Data;

import java.time.LocalDate;
import java.time.LocalDateTime;

@Data
public class SedeResumenDTO {
    private Integer idSede;
    private String nombre;
    private String direccion;
    private int capacidad;
    private LocalDate fechaCreacion;
    private boolean cubierta;
    private double costoMantenimiento;
    //dos de la clase a
    private String idEventoAsociado;
    private String nombreEventoAsociado;
}
