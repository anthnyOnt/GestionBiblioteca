Feature: Insertar Lector Unhappy Path

Como bibliotecario
Quiero que el sistema valide los datos obligatorios al registrar lectores
Para garantizar la integridad de la información en el sistema

Background:
    Given que soy un usuario autorizado del sistema

Scenario: Registro de lector con correo vacío debe ser rechazado
    Given que no existe un lector con CI "12340000"
    When intento registrar un lector con los siguientes datos:
        | Campo           | Valor    |
        | CI              | 12340000 |
        | PrimerNombre    | Luis     |
        | SegundoNombre   | Manuel   |
        | PrimerApellido  | Suarez   |
        | SegundoApellido | Loza     |
        | Telefono        | 79999999 |
        | Correo          |          |
    Then el sistema rechaza la operacion con el mensaje "El correo electrónico es obligatorio."
    And no se crea ningún lector en el sistema