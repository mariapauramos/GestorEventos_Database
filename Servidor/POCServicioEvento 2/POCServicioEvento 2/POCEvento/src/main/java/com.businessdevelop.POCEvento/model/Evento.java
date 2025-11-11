package com.businessdevelop.POCEvento.model;

import com.fasterxml.jackson.annotation.JsonFormat;
import jakarta.persistence.Column;
import jakarta.persistence.Id;
import jakarta.persistence.MappedSuperclass;
import lombok.Data;
import lombok.NoArgsConstructor;
import lombok.ToString;
import java.time.LocalDate;

@Data
@NoArgsConstructor
@ToString
@MappedSuperclass
public abstract class Evento {

    @Id
    @Column(name = "id_evento")
    private String idEvento;

    @Column(nullable = false)
    private String nombre;

    @Column(nullable = false)
    private String ciudad;

    @Column(nullable = false)
    private int asistentes;

    @JsonFormat(pattern = "yyyy-MM-dd")
    @Column(nullable = false)
    private LocalDate fecha;

    public Evento(String idEvento, String nombre, String ciudad, int asistentes,
                  LocalDate fecha) {
        this.idEvento = idEvento;
        this.nombre = nombre;
        this.ciudad = ciudad;
        this.asistentes = asistentes;
        this.fecha = fecha;
    }

}