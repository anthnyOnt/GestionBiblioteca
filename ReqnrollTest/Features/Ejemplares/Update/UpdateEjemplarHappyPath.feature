Feature: Actualizar Ejemplar Path Exitoso
  Como bibliotecario
  Quiero actualizar la información de ejemplares existentes
  Para poder mantener detalles precisos de las copias de libros

  Background:
    Given que tengo acceso al sistema de gestión de biblioteca para ejemplares update happy
    And que existen ejemplares que pueden ser modificados para update happy

  Scenario: Actualizar exitosamente ejemplar con datos válidos - Caso 1
    Given que existe un ejemplar que puede ser actualizado para ejemplares update happy
    When actualizo el ejemplar con los siguientes datos válidos para ejemplares update happy:
      | Campo            | Valor                |
      | Descripcion      | Ejemplar Actualizado |
      | Observaciones    | Revisado             |
      | Disponible       | true                 |
    And envío el formulario de actualización del ejemplar para ejemplares update happy
    Then el ejemplar debe actualizarse exitosamente en la base de datos para ejemplares update happy
    And el sistema debe retornar una respuesta exitosa de actualización para ejemplares update happy

  Scenario: Actualizar exitosamente ejemplar con datos válidos - Caso 2
    Given que existe un ejemplar que puede ser actualizado para ejemplares update happy
    When actualizo el ejemplar con los siguientes datos válidos para ejemplares update happy:
      | Campo            | Valor              |
      | Descripcion      | Ejemplar Renovado  |
      | Observaciones    | En perfecto estado |
      | Disponible       | false              |
    And envío el formulario de actualización del ejemplar para ejemplares update happy
    Then el ejemplar debe actualizarse exitosamente en la base de datos para ejemplares update happy
    And el sistema debe retornar una respuesta exitosa de actualización para ejemplares update happy
