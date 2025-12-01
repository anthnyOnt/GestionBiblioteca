Feature: Insertar Lector Happy Path

Como bibliotecario
Quiero registrar nuevos lectores en el sistema
Para que puedan solicitar préstamos de libros

Background:
    Given que soy un usuario autorizado del sistema

Scenario Outline: Registrar nuevo lector con datos válidos
    Given que no existe un lector con CI "<ci>"
    When registro un nuevo lector con los siguientes datos:
        | Campo            | Valor              |
        | CI               | <ci>               |
        | PrimerNombre     | <primerNombre>     |
        | SegundoNombre    | <segundoNombre>    |
        | PrimerApellido   | <primerApellido>   |
        | SegundoApellido  | <segundoApellido>  |
        | Telefono         | <telefono>         |
        | Correo           | <correo>           |
    Then el lector se guarda correctamente
    And el sistema asigna un ID único al lector
    And el lector queda con estado "Activo"
    And el lector tiene rol "LECTOR"

Examples:
    | ci       | primerNombre | segundoNombre | primerApellido | segundoApellido | telefono | correo                    |
    | 1234567  | Juan         | Carlos        | Perez          | Gutierrez       | 77777777 | juan.perez@gmail.com      |
    | 87654321 | Maria        | Elena         | Fernandez      | Rojas           | 78888888 | maria.fernandez@gmail.com |


