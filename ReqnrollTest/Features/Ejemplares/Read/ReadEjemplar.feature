Feature: Consultar Ejemplar
  Como bibliotecario
  Quiero recuperar información del ejemplar por ID
  Para poder ver y gestionar los detalles específicos de las copias de libros

  Background:
    Given que tengo acceso al sistema de gestión de biblioteca para ejemplares read
    And que existen ejemplares almacenados en el sistema para ejemplares read

  Scenario: Recuperar exitosamente datos del ejemplar por ID
    Given que existe un ejemplar con datos válidos para ejemplares read
    When solicito la información del ejemplar por ID para ejemplares read: 1
    Then debo recibir los detalles completos del ejemplar para ejemplares read
    And los datos del ejemplar retornados deben coincidir con la información almacenada para ejemplares read