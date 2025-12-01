Feature: Validación de Errores en Registro de Libros

Como bibliotecario
Quiero que el sistema valide los datos al registrar un libro
Para evitar información incorrecta en el catálogo

  Background:
    Given que soy un usuario autorizado del sistema para libros

  Scenario: Registrar libro sin título debe fallar
    Given que no existe un libro con ISBN "9781234567890"
    When intento registrar un libro con los siguientes datos:
        | Campo            | Valor           |
        | Titulo           |                 |
        | ISBN             | 9781234567890   |
        | Idioma           | Español         |
        | Edicion          | 1ra edición     |
        | FechaPublicacion | 2020-01-01      |
        | Sinopsis         | Libro sin título|
    Then el sistema rechaza la operación con el mensaje de libro "El título es obligatorio"
    And no se crea ningún libro en el sistema
