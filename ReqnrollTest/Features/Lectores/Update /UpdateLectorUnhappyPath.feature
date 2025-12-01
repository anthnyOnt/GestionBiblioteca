Feature: Validación de Errores en Actualización de Lector

Como bibliotecario
Quiero que el sistema valide los datos al actualizar un lector
Para evitar información incorrecta en la base de datos

Background:
    Given que soy un usuario autorizado del sistema

Scenario: Actualizar lector con CI vacío debe fallar
    When intento actualizar la información de un lector con CI vacío
    Then el sistema rechaza la operación con el mensaje "El número de cédula es obligatorio"
    And no se realiza ningún cambio en el sistema

