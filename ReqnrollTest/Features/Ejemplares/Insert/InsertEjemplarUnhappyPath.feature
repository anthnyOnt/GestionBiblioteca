Feature: Insertar Ejemplar Path No Exitoso
  Como bibliotecario
  Quiero recibir errores de validación claros cuando proporciono datos inválidos del ejemplar
  Para poder corregir la información y asegurar la integridad de los datos

  Background:
    Given que tengo acceso al sistema de gestión de biblioteca para ejemplares insert unhappy
    And que existen libros disponibles en el catálogo para ejemplares insert unhappy

  Scenario: Falla al crear ejemplar con campos requeridos faltantes
    Given que quiero crear un nuevo ejemplar para ejemplares insert unhappy
    When proporciono los siguientes datos inválidos del ejemplar para ejemplares insert unhappy:
      | Campo            | Valor      |
      | IdLibro          | null       |
      | Descripcion      | Test Desc  |
      | Observaciones    | Good       |
      | Disponible       | true       |
      | FechaAdquisicion | 2024-01-15 |
    And envío el formulario de creación del ejemplar para ejemplares insert unhappy
    Then la creación del ejemplar debe fallar para ejemplares insert unhappy
    And debo recibir un mensaje de error de validación para ejemplares insert unhappy: "Debe seleccionar un libro"
