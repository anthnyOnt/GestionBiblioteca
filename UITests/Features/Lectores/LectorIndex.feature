Feature: Operaciones en la página de índice de lectores
    Como usuario del sistema
    Quiero poder ver, seleccionar todos y eliminar lectores desde la página de índice
    Para gestionar eficientemente los lectores de la biblioteca

Background:
    Given que estoy en la página de índice de lectores

Scenario: Seleccionar todos los lectores de la lista
    When selecciono todos los lectores de la lista
    Then todos los lectores deberían estar seleccionados

Scenario: Eliminar un lector desde la lista
    When hago clic en eliminar para un lector
    And confirmo la eliminación
    Then el lector debería ser eliminado exitosamente
    And debería redirigir a la página de índice
