package com.businessdevelop.POCEvento.controller;

import com.businessdevelop.POCEvento.dto.EventoConSedeDTO;
import com.businessdevelop.POCEvento.model.Evento;
import com.businessdevelop.POCEvento.model.Equipo;
import com.businessdevelop.POCEvento.model.EventoDeportivo;
import com.businessdevelop.POCEvento.servicio.ServicioEvento;
import jakarta.validation.Valid;
import org.springframework.http.ResponseEntity;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/eventos")
public class ControladorEvento {

    private final ServicioEvento servicioEvento;

    public ControladorEvento(ServicioEvento servicioEvento) {
        this.servicioEvento = servicioEvento;
    }

    @GetMapping("/healthCheck")
    public String healthCheck() {

        return "Status ok para evento deportivo!";
    }

    //Crear ED
    @PostMapping("/")
    public ResponseEntity<?> crearED(@Valid @RequestBody EventoDeportivo evento) {
        try {
            EventoDeportivo creado = servicioEvento.crearEvento(evento);
            return ResponseEntity.status(HttpStatus.CREATED).body(creado);

        } catch (IllegalArgumentException e) {
            return ResponseEntity.status(HttpStatus.CONFLICT).body(e.getMessage());
        }
    }

    //Buscar ED
    @GetMapping("/{idEvento}")
    public ResponseEntity<Object> buscarEvento(@PathVariable String idEvento) {
        return servicioEvento.buscarEvento(idEvento)
                .<ResponseEntity<Object>>map(e -> ResponseEntity.ok(e))
                .orElseGet(() -> ResponseEntity.status(HttpStatus.NOT_FOUND)
                        .body("No existe un evento con id: " + idEvento));
    }


    //Listar todos los eventos con equipos
    @GetMapping("/")
    public ResponseEntity<List<EventoDeportivo>> listarDeportivos() {
        return ResponseEntity.ok(servicioEvento.listarEventosConEquipos());
    }

    //Listar con filtro (ciudad y tipo de deporte)
    @GetMapping("/filtrar")
    public ResponseEntity<?> filtrarED(
            @RequestParam(required = false) String ciudad,
            @RequestParam(required = false) String tipoDeporte) {

        if (ciudad == null && tipoDeporte == null) {
            return ResponseEntity.badRequest()
                    .body("Debes ingresar al menos un filtro: ciudad o tipoDeporte");
        }

        return ResponseEntity.ok(servicioEvento.listarFiltroED(ciudad, tipoDeporte));
    }

    //Actualizar evento
    @PutMapping("/{idEvento}")
    public ResponseEntity<?> actualizarED(
            @PathVariable String idEvento,
            @RequestBody EventoDeportivo eventoActualizado) {

        boolean actualizado = servicioEvento.actualizarEvento(idEvento, eventoActualizado);

        return actualizado ?
                ResponseEntity.ok("Evento deportivo actualizado") :
                ResponseEntity.status(HttpStatus.NOT_FOUND).body("Evento no encontrado");
    }

    //Eliminar evento
    @DeleteMapping("/{idEvento}")
    public ResponseEntity<?> eliminarEvento(@PathVariable String idEvento) {
        boolean eliminado = servicioEvento.eliminarEvento(idEvento);

        return eliminado ?
                ResponseEntity.ok("Evento eliminado correctamente") :
                ResponseEntity.status(HttpStatus.NOT_FOUND).body("Evento no encontrado");
    }

    @GetMapping("/{idEvento}/detallado")
    public ResponseEntity<?> buscarEventoDetallado(@PathVariable String idEvento) {
        try {
            EventoConSedeDTO dto = servicioEvento.buscarEventoConSede(idEvento);
            return ResponseEntity.ok(dto);
        } catch (RuntimeException e) {
            return ResponseEntity.status(HttpStatus.NOT_FOUND)
                    .body(e.getMessage());
        }
    }
}
