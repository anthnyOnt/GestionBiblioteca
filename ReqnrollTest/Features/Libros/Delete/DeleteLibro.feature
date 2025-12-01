Feature: Eliminar Libro del Sistema

Como bibliotecario
Quiero eliminar un libro del sistema
Para mantener actualizado el catálogo de la biblioteca

  Background:
    Given que soy un usuario autorizado del sistema para libros

  Scenario: Eliminar libro existente exitosamente
    Given que existe un libro con ISBN "9781234567890" y título "El Principito"
    When elimino el libro con ISBN "9781234567890"
    Then el libro se elimina correctamente
    And el sistema devuelve estado HTTP para eliminación de libro 204
    And el libro ya no existe en el sistema