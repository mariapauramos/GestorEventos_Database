package com.businessdevelop.POCEvento.controller;
import com.businessdevelop.POCEvento.model.Equipo;
import com.businessdevelop.POCEvento.servicio.ServicioEquipo;
import jakarta.validation.Valid;
import org.springframework.http.ResponseEntity;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/equipos")
public class ControladorEquipo {

    private final ServicioEquipo servicioEquipo;

    public ControladorEquipo(ServicioEquipo servicioEquipo) {
        this.servicioEquipo = servicioEquipo;
    }

    @GetMapping("/healthCheck")
    public String healthCheck() {
        return "Status ok para equipo!";
    }

    //Crear equipo
    @PostMapping("/")
    public ResponseEntity<Object> crearEquipo(@Valid @RequestBody Equipo equipo) {
        try {
            Equipo creado = servicioEquipo.crearEquipo(equipo);
            return ResponseEntity.status(HttpStatus.CREATED).body(creado);

        } catch (IllegalArgumentException e) {
            return ResponseEntity.status(HttpStatus.CONFLICT).body(e.getMessage());
        }
    }

    //buscar equipos
    @GetMapping("/{idEquipo}")
    public ResponseEntity<Object> buscarEquipo(@PathVariable Integer idEquipo) {
        return servicioEquipo.buscarEquipo(idEquipo)
                .<ResponseEntity<Object>>map(ResponseEntity::ok)
                .orElseGet(() -> ResponseEntity.status(HttpStatus.NOT_FOUND)
                        .body("No existe un equipo con id: " + idEquipo));
    }

    //listar equipos
    @GetMapping("/")
    public ResponseEntity<List<Equipo>> listarEquipos() {

        return ResponseEntity.ok(servicioEquipo.listarEquipos());
    }

    //Filtrar por numero de jugadores
    @GetMapping("/filtro")
    public ResponseEntity<Object> listarPorNumeroJugadores(
            @RequestParam(required = false) Integer numeroJugadores) {

        if (numeroJugadores == null) {
            return ResponseEntity.badRequest()
                    .body("Debes ingresar el filtro: numeroJugadores");
        }

        return ResponseEntity.ok(servicioEquipo.listarPorNumeroJugadores(numeroJugadores));
    }

    //actualizar equipo
    @PutMapping(value = "/{idEquipo}", consumes = "application/json", produces = "application/json")
    public ResponseEntity<Object> actualizarEquipo(
            @PathVariable Integer idEquipo,
            @RequestBody Equipo equipoActualizado) {
        try {
            boolean actualizado = servicioEquipo.actualizarEquipo(idEquipo, equipoActualizado);

            if (actualizado) {
                return ResponseEntity.ok("Equipo actualizado correctamente");
            } else {
                return ResponseEntity.status(HttpStatus.NOT_FOUND)
                        .body("No existe un equipo con id: " + idEquipo);
            }
        } catch (Exception e) {
            return ResponseEntity.status(HttpStatus.BAD_REQUEST)
                    .body("Error al actualizar el equipo: " + e.getMessage());
        }
    }


    //eliminar equipo
    @DeleteMapping("/{idEquipo}")
    public ResponseEntity<Object> eliminarEquipo(@PathVariable Integer idEquipo) {
        boolean eliminado = servicioEquipo.eliminarEquipo(idEquipo);

        return eliminado ?
                ResponseEntity.ok("Equipo eliminado correctamente") :
                ResponseEntity.status(HttpStatus.NOT_FOUND)
                        .body("No existe un equipo con id: " + idEquipo);
    }
}
