Feature: Actualizar Información de Libro

Como bibliotecario
Quiero actualizar la información de un libro registrado en el sistema
Para corregir errores o actualizar información nueva

  Background:
    Given que soy un usuario autorizado del sistema para libros

  Scenario Outline: Actualizar datos de libro exitosamente
    Given que existe un libro con ISBN "<ISBN>" y <campo_original> "<valor_original>"
    When actualizo su <campo_original> a "<valor_nuevo>"
    Then los datos del libro se actualizan correctamente
    And el sistema devuelve estado HTTP para actualización de libro 200
    And el libro tiene <campo_original> "<valor_nuevo>"

Examples:
    | ISBN          | campo_original | valor_original | valor_nuevo     |
    | 9781234567890 | edición        | 1ra edición    | 2da edición     |
    | 9780060883287 | idioma         | Español        | Inglés          |