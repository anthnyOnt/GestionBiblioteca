Feature: Consultar Información de Lector

Como bibliotecario
Quiero consultar la información de un lector registrado en el sistema
Para verificar sus datos personales y estado de membresía

Background:
    Given que soy un usuario autorizado del sistema

Scenario: Consultar lector existente por CI
    Given que existe un lector con CI "12345678", nombre "Juan Pérez" y correo "juan.perez@email.com"
    When consulto la información del lector con CI "12345678"
    Then el sistema muestra la información completa del lector:
        | Campo          | Valor                    |
        | CI             | 12345678                 |
        | Nombre         | Juan Pérez               |
        | Correo         | juan.perez@email.com     |
        | Estado         | Activo                   |