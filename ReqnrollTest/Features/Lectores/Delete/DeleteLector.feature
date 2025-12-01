Feature: Eliminar Lector del Sistema

Como bibliotecario
Quiero eliminar un lector del sistema
Para mantener actualizada la base de datos de usuarios

Background:
    Given que soy un usuario autorizado del sistema

Scenario: Eliminar lector existente exitosamente
    Given que existe un lector con CI "12345678" y nombre "María García"
    When elimino el lector con CI "12345678"
    Then el lector se elimina correctamente
    And el sistema devuelve estado HTTP para eliminación 204
    And el lector ya no existe en el sistema