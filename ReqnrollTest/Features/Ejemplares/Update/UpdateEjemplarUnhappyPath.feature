Feature: Actualizar Ejemplar Path No Exitoso
  Como bibliotecario
  Quiero recibir errores de validación claros al actualizar ejemplar con datos inválidos
  Para poder corregir la información y mantener la integridad de los datos

  Background:
    Given que tengo acceso al sistema de gestión de biblioteca para ejemplares update unhappy
    And que existen ejemplares que pueden ser modificados para update unhappy

  Scenario: Falla al actualizar ejemplar con datos inválidos
    Given que existe un ejemplar que puede ser actualizado para ejemplares update unhappy
    When intento actualizar el ejemplar con datos inválidos para ejemplares update unhappy:
      | Campo         | Valor |
      | Descripcion   | null  |
      | Observaciones | Valid |
    And envío el formulario de actualización del ejemplar para ejemplares update unhappy
    Then la actualización del ejemplar debe fallar para ejemplares update unhappy
    And debo recibir un mensaje de error de validación para ejemplares update unhappy: "La descripción es obligatoria"