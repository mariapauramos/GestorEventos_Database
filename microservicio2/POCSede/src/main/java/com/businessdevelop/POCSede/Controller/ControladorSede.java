package com.businessdevelop.POCSede.Controller;


import jakarta.validation.Valid;
import com.businessdevelop.POCSede.model.SedeDeportiva;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import com.businessdevelop.POCSede.servicio.ServicioSede;

import java.util.List;

@RestController
@RequestMapping("/sedes")
public class ControladorSede {

    private final ServicioSede servicio;

    public ControladorSede(ServicioSede servicio) {
        this.servicio = servicio;
    }

    @GetMapping("/healthCheck")
    public String healthCheck() {
        return "Status ok para sede deportiva!";
    }

    // Crear
    @PostMapping("/")
    public ResponseEntity<SedeDeportiva> crear(@Valid @RequestBody SedeDeportiva sede) {
        SedeDeportiva creada = servicio.crear(sede);
        return ResponseEntity.status(HttpStatus.CREATED).body(creada);
    }

    // Buscar individual
    @GetMapping("/{id}")
    public ResponseEntity<?> buscarPorId(@PathVariable Integer id) {
        return servicio.buscarPorId(id)
                .<ResponseEntity<?>>map(ResponseEntity::ok)
                .orElseGet(() -> ResponseEntity
                        .status(HttpStatus.NOT_FOUND)
                        .body("No existe una sede con id: " + id));
    }

    // Listar todas
    @GetMapping("/")
    public ResponseEntity<List<SedeDeportiva>> listarTodas() {
        return ResponseEntity.ok(servicio.listarTodas());
    }

    // Listar por dirección (variante 1)
    @GetMapping("/buscarPorDireccion")
    public ResponseEntity<List<SedeDeportiva>> listarPorDireccion(
            @RequestParam String texto) {
        return ResponseEntity.ok(servicio.listarPorDireccion(texto));
    }

    // Listar por capacidad mínima (variante 2)
    @GetMapping("/buscarPorCapacidad")
    public ResponseEntity<List<SedeDeportiva>> listarPorCapacidad(
            @RequestParam int capacidadMinima) {
        return ResponseEntity.ok(servicio.listarPorCapacidadMinima(capacidadMinima));
    }

    // Actualizar
    @PutMapping("/{id}")
    public ResponseEntity<?> actualizar(
            @PathVariable Integer id,
            @RequestBody SedeDeportiva sedeActualizada) {

        boolean ok = servicio.actualizar(id, sedeActualizada);
        return ok
                ? ResponseEntity.ok("Sede deportiva actualizada correctamente")
                : ResponseEntity.status(HttpStatus.NOT_FOUND)
                .body("No existe una sede con id: " + id);
    }

    // Eliminar
    @DeleteMapping("/{id}")
    public ResponseEntity<?> eliminar(@PathVariable Integer id) {
        boolean eliminado = servicio.eliminar(id);
        return eliminado
                ? ResponseEntity.ok("Sede deportiva eliminada correctamente")
                : ResponseEntity.status(HttpStatus.NOT_FOUND)
                .body("No existe una sede con id: " + id);
    }

    @GetMapping("/porEvento/{idEvento}")
    public ResponseEntity<List<SedeDeportiva>> buscarPorEvento(@PathVariable String idEvento) {
        List<SedeDeportiva> sedes = servicio.buscarPorEventoAsociado(idEvento);
        return ResponseEntity.ok(sedes);
    }




}
