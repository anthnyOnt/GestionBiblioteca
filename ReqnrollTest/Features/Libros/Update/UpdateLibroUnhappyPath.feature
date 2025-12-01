Feature: Validación de Errores en Actualización de Libro

Como bibliotecario
Quiero que el sistema valide los datos al actualizar un libro
Para evitar información incorrecta en el catálogo

  Background:
    Given que soy un usuario autorizado del sistema para libros

  Scenario: Actualizar libro con título vacío debe fallar
    Given que existe un libro con ISBN "9781234567890" para modificación
    When intento actualizar el título a una cadena vacía para fallo de validación
    Then el sistema rechaza la actualización por validación
    And el sistema devuelve estado HTTP de error para actualización 400
    And los datos del libro no se modifican por validación fallida