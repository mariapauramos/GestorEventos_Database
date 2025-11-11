/**
 *
 * @author mariaramos
 */
package com.businessdevelop.POCEvento.model;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonManagedReference;
import jakarta.persistence.*;
import lombok.*;
import com.fasterxml.jackson.annotation.JsonIgnore;
import java.time.LocalDate;
import java.util.ArrayList;
import java.util.List;

@Entity
@Table(name = "EventoDeportivo")
@Data
@NoArgsConstructor
@EqualsAndHashCode(callSuper = true)
@ToString(callSuper = true)
public class EventoDeportivo extends Evento {

    @Column(nullable = false)
    private String tipoDeporte;

    @OneToMany(mappedBy = "eventoDeportivo", cascade = {CascadeType.PERSIST, CascadeType.MERGE})
    //Evita serializar la lista cuando muestras un evento individual
    @JsonManagedReference
    @JsonIgnoreProperties({"eventoDeportivo", "ciudadOrigen", "numeroJugadores", "puntaje"})
    private List<Equipo> equipos;



    public EventoDeportivo(String idEvento, String nombre, String ciudad, int asistentes,
                           LocalDate fecha, String tipoDeporte) {
        super(idEvento, nombre, ciudad, asistentes, fecha);
        this.tipoDeporte = tipoDeporte;
    }
}




