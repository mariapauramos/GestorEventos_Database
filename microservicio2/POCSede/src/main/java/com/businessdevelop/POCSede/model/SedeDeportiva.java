package com.businessdevelop.POCSede.model;


import jakarta.persistence.*;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.Positive;
import jakarta.validation.constraints.Size;
import lombok.Data;
import lombok.NoArgsConstructor;
import jakarta.persistence.Id;

import java.time.LocalDateTime;

@Entity
@Table(name = "sede_deportiva")
@Data
@NoArgsConstructor
public class SedeDeportiva {

    @Id
    //@GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "id_sede")
    private Integer idSede;

    @NotBlank
    @Size(max = 100)
    @Column(nullable = false)
    private String nombre;

    @Positive
    @Column(nullable = false)
    private int capacidad;              // int

    @NotBlank
    @Size(max = 200)
    @Column(nullable = false)
    private String direccion;           // String

    @Positive
    @Column(nullable = false)
    private double costoMantenimiento;  // double

    @Column(nullable = false)
    private LocalDateTime fechaCreacion; // LocalDateTime

    @Column(nullable = false)
    private boolean cubierta;           // boolean

    //Conexión lógica al EventoDeportivo (Clase A) del otro microservicio
    @NotBlank
    @Column(name = "id_evento_asociado", nullable = false, length = 50)
    private String idEventoAsociado;


}
