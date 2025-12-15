Feature: Operaciones en la página de índice de libros
    Como usuario del sistema
    Quiero poder ver, seleccionar todos y eliminar libros desde la página de índice
    Para gestionar eficientemente los libros de la biblioteca

Background:
    Given que estoy en la página de índice de libros

Scenario: Seleccionar todos los libros de la lista
    When selecciono todos los libros de la lista
    Then todos los libros deberían estar seleccionados

Scenario: Eliminar un libro desde la lista
    When hago clic en eliminar para un libro
    And confirmo la eliminación
    Then el libro debería ser eliminado exitosamente
    And debería redirigir a la página de índice
