Feature: Operaciones en la página de índice de ejemplares
    Como usuario del sistema
    Quiero poder ver, seleccionar todos y eliminar ejemplares desde la página de índice
    Para gestionar eficientemente los ejemplares de la biblioteca

Background:
    Given que estoy en la página de índice de ejemplares

Scenario: Seleccionar todos los ejemplares de la lista
    When selecciono todos los ejemplares de la lista
    Then todos los ejemplares deberían estar seleccionados

Scenario: Eliminar un ejemplar desde la lista
    When hago clic en eliminar para un ejemplar
    And confirmo la eliminación
    Then el ejemplar debería ser eliminado exitosamente
    And debería redirigir a la página de índice
