package com.businessdevelop.POCSede.repositories;

import com.businessdevelop.POCSede.model.SedeDeportiva;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;
import java.util.Optional;

public interface SedeRepository extends JpaRepository<SedeDeportiva, Integer> {

    // Variante 1 de búsqueda en lista: por ciudad (dirección contiene ciudad)
    List<SedeDeportiva> findByDireccionContainingIgnoreCase(String texto);

    // Variante 2 de búsqueda en lista: por capacidad mínima
    List<SedeDeportiva> findByCapacidadGreaterThanEqual(int capacidad);


    List<SedeDeportiva> findByIdEventoAsociado(String idEvento);

}
