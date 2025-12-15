Feature: Prestamo Creation
  Como bibliotecario
  Quiero crear préstamos para los lectores
  Para gestionar el préstamo de ejemplares

  Background:
    Given el sistema tiene un usuario con CI "1234567" registrado
    And el sistema tiene un libro con 3 ejemplares disponibles

  Scenario: CP1 - Crear préstamo con un solo ejemplar
    When navego a la página de crear préstamo
    And busco el lector con CI "1234567"
    And busco el libro por título
    And agrego el ejemplar 1 al préstamo
    And establezco la fecha límite del ejemplar 1 para 7 días después
    And confirmo el préstamo
    Then el préstamo debe ser creado exitosamente
    And debo ser redirigido a la página de préstamos

  Scenario: CP2 - Crear préstamo con dos ejemplares del mismo libro
    When navego a la página de crear préstamo
    And busco el lector con CI "1234567"
    And busco el libro por título
    And agrego el ejemplar 1 al préstamo
    And agrego el ejemplar 2 al préstamo
    And establezco la fecha límite del ejemplar 1 para 7 días después
    And establezco la fecha límite del ejemplar 2 para 7 días después
    And confirmo el préstamo
    Then el préstamo debe ser creado exitosamente
    And debo ser redirigido a la página de préstamos

  Scenario: CP3 - Crear préstamo con tres ejemplares con fechas límite diferentes
    When navego a la página de crear préstamo
    And busco el lector con CI "1234567"
    And busco el libro por título
    And agrego el ejemplar 1 al préstamo
    And agrego el ejemplar 2 al préstamo
    And agrego el ejemplar 3 al préstamo
    And establezco la fecha límite del ejemplar 1 para 7 días después
    And establezco la fecha límite del ejemplar 2 para 14 días después
    And establezco la fecha límite del ejemplar 3 para 21 días después
    And confirmo el préstamo
    Then el préstamo debe ser creado exitosamente
    And debo ser redirigido a la página de préstamos
