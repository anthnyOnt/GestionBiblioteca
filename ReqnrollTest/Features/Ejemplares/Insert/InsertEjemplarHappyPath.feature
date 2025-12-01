Feature: Insertar Nuevo Ejemplar en el Sistema

Como bibliotecario
Quiero agregar nuevos ejemplares al sistema  
Para poder gestionar las copias de libros disponibles para préstamo

  Background:
    Given que tengo acceso al sistema de gestión de biblioteca para ejemplares insert
    And que existen libros disponibles en el catálogo para ejemplares insert

  Scenario: Crear exitosamente un nuevo ejemplar con datos válidos - Caso 1
    Given que quiero crear un nuevo ejemplar para ejemplares insert
    When proporciono los siguientes datos válidos del ejemplar para ejemplares insert:
      | Campo            | Valor              |
      | IdLibro          | 1                  |
      | Descripcion      | Ejemplar de Prueba |
      | Observaciones    | En buen estado     |
      | Disponible       | true               |
      | FechaAdquisicion | 2024-01-15         |
    And envío el formulario de creación del ejemplar para ejemplares insert
    Then el ejemplar debe crearse exitosamente en la base de datos para ejemplares insert
    And el sistema debe retornar una respuesta exitosa de creación para ejemplares insert

  Scenario: Crear exitosamente un nuevo ejemplar con datos válidos - Caso 2
    Given que quiero crear un nuevo ejemplar para ejemplares insert
    When proporciono los siguientes datos válidos del ejemplar para ejemplares insert:
      | Campo            | Valor            |
      | IdLibro          | 1                |
      | Descripcion      | Copia Principal  |
      | Observaciones    | Recién adquirido |
      | Disponible       | false            |
      | FechaAdquisicion | 2024-02-01       |
    And envío el formulario de creación del ejemplar para ejemplares insert
    Then el ejemplar debe crearse exitosamente en la base de datos para ejemplares insert
    And el sistema debe retornar una respuesta exitosa de creación para ejemplares insert
