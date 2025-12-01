Feature: Actualizar Información de Lector

Como bibliotecario
Quiero actualizar la información de un usuario registrado en el sistema
Para poder corregir errores o actualizar información nueva

Background:
    Given que soy un usuario autorizado del sistema

Scenario Outline: Actualizar datos de lector exitosamente
    Given que existe un lector con CI "<CI>" y <campo_original> "<valor_original>"
    When actualizo su <campo_original> a "<valor_nuevo>"
    Then los datos se actualizan correctamente
    And el sistema devuelve estado HTTP para actualización 200
    And el lector tiene <campo_original> "<valor_nuevo>"

Examples:
    | CI       | campo_original | valor_original   | valor_nuevo      |
    | 12345678 | teléfono       | 70000000         | 77777777         |
    | 87654321 | correo         | old@example.com  | nuevo@example.com|