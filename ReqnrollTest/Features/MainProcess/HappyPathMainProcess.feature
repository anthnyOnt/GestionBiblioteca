Feature: Main Process Happy Path

Ejecucion del proceso principal en un escenario de Happy Path
(Prestamo de ejemplares creado exitosamente)

Scenario: Main Process
    Given que existe un lector con CI "12345678" y estado "Activo"
    And existe un libro con Título "El Principito" y 2 ejemplares disponibles
    When registró un préstamo para el lector con CI "12345678" seleccionando los ejemplares con Ids "EJ-101" y "EJ-102"
    Then el sistema confirma el préstamo y marca los ejemplares como "Prestado"
    And se crea el registro de préstamo con estado "Activo"