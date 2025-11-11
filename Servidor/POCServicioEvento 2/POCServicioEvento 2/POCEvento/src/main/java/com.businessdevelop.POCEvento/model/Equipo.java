/**
 *
 * @author mariaramos
 */

package com.businessdevelop.POCEvento.model;

import com.fasterxml.jackson.annotation.*;
import jakarta.persistence.*;
import lombok.*;

@Entity
@Table(name = "equipo")
@Data
@NoArgsConstructor
@AllArgsConstructor
public class Equipo {

    @Id  // @GeneratedValue
    private Integer idEquipo;

    @Column(nullable = false)
    private String nombre;

    @Column(nullable = false)
    private String ciudadOrigen;

    @Column(nullable = false)
    private int numeroJugadores;

    @Column(nullable = false)
    private double puntaje;

    @ManyToOne
    @JoinColumn(name = "fkevento_deportivo")
    @JsonBackReference
    private EventoDeportivo eventoDeportivo;

    @JsonProperty("nombreEvento")
    public String getNombreEvento() {
        return (eventoDeportivo != null) ? eventoDeportivo.getNombre() : null;
    }

}