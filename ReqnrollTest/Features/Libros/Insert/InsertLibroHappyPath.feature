Feature: Registrar Nuevo Libro en el Sistema

Como bibliotecario
Quiero registrar libros nuevos en el sistema
Para mantener actualizado el catálogo de la biblioteca

  Background:
    Given que soy un usuario autorizado del sistema para libros

  Scenario Outline: Registrar libro con datos válidos
    Given que no existe un libro con ISBN "<ISBN>"
    When registro un nuevo libro con los siguientes datos:
        | Campo            | Valor                |
        | Titulo           | <Titulo>             |
        | ISBN             | <ISBN>               |
        | Idioma           | <Idioma>             |
        | Edicion          | <Edicion>            |
        | FechaPublicacion | <FechaPublicacion>   |
        | Sinopsis         | <Sinopsis>           |
    Then el libro se guarda correctamente en el sistema
    And el sistema asigna un ID único al libro
    And el libro queda con estado "Activo"

Examples:
    | Titulo                | ISBN          | Idioma  | Edicion     | FechaPublicacion | Sinopsis                        |
    | El Principito         | 9781234567890 | Español | 1ra edición | 1943-04-06       | Un cuento poético               |
    | Cien Años de Soledad  | 9780060883287 | Español | 2da edición | 1967-05-30       | Historia de la familia Buendía  |