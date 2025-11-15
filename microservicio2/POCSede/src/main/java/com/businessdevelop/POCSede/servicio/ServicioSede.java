package com.businessdevelop.POCSede.servicio;


import jakarta.transaction.Transactional;
import com.businessdevelop.POCSede.model.SedeDeportiva;
import org.springframework.stereotype.Service;
import com.businessdevelop.POCSede.repositories.SedeRepository;


import java.time.LocalDateTime;
import java.util.List;
import java.util.Optional;

@Service
public class ServicioSede {

    private final SedeRepository repositorio;

    public ServicioSede(
            SedeRepository repositorio) {
        this.repositorio = repositorio;
    }

    @Transactional
    public SedeDeportiva crear(SedeDeportiva sede) {

        // setear fecha de creación si viene nula
        if (sede.getFechaCreacion() == null) {
            sede.setFechaCreacion(LocalDateTime.now());
        }

        // aquí podrías validar contra el microservicio de EventoDeportivo:
        // llamar a http://localhost:8091/eventos/{idEventoAsociado}
        // y lanzar excepción si no existe

        return repositorio.save(sede);
    }

    // Buscar individual
    public Optional<SedeDeportiva> buscarPorId(Integer id) {
        return repositorio.findById(id);
    }

    // Listar todas
    public List<SedeDeportiva> listarTodas() {
        return repositorio.findAll();
    }

    // Listar por dirección (contiene texto)
    public List<SedeDeportiva> listarPorDireccion(String texto) {
        return repositorio.findByDireccionContainingIgnoreCase(texto);
    }

    // Listar por capacidad mínima
    public List<SedeDeportiva> listarPorCapacidadMinima(int capacidad) {
        return repositorio.findByCapacidadGreaterThanEqual(capacidad);
    }

    // Actualizar
    @Transactional
    public boolean actualizar(Integer id, SedeDeportiva actualizada) {
        return repositorio.findById(id).map(existente -> {

            existente.setNombre(actualizada.getNombre());
            existente.setCapacidad(actualizada.getCapacidad());
            existente.setDireccion(actualizada.getDireccion());
            existente.setCostoMantenimiento(actualizada.getCostoMantenimiento());
            existente.setCubierta(actualizada.isCubierta());
            existente.setIdEventoAsociado(actualizada.getIdEventoAsociado());

            // fechaCreacion normalmente no se cambia, pero si el profe lo pide:
            if (actualizada.getFechaCreacion() != null) {
                existente.setFechaCreacion(actualizada.getFechaCreacion());
            }

            repositorio.save(existente);
            return true;
        }).orElse(false);
    }

    // Eliminar
    @Transactional
    public boolean eliminar(Integer id) {
        if (repositorio.existsById(id)) {
            repositorio.deleteById(id);
            return true;
        }
        return false;
    }

    public Optional<SedeDeportiva> buscarPorEventoAsociado(String idEvento) {
        return repositorio.findByIdEventoAsociado(idEvento);
    }
}
