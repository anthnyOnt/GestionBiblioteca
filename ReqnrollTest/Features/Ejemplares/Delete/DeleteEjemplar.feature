Feature: Eliminar Ejemplar
  Como bibliotecario
  Quiero eliminar ejemplares del sistema
  Para poder remover copias de libros obsoletas o dañadas

  Background:
    Given que tengo acceso al sistema de gestión de biblioteca para ejemplares delete
    And que existen ejemplares que pueden ser eliminados

  Scenario: Eliminar exitosamente un ejemplar existente
    Given que existe un ejemplar que puede ser eliminado para ejemplares delete
    When solicito eliminar el ejemplar por ID para ejemplares delete: 1
    Then el ejemplar debe eliminarse exitosamente para ejemplares delete
    And el sistema debe retornar una respuesta exitosa de eliminación para ejemplares delete
    And el ejemplar debe marcarse como inactivo en la base de datos para ejemplares delete