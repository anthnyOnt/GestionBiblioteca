Feature: Actualización de Ejemplares a través de la UI
    Como usuario del sistema
    Quiero actualizar ejemplares existentes a través de la interfaz de usuario
    Para mantener actualizada la información del inventario de la biblioteca

Scenario Outline: Actualizar ejemplar con diferentes combinaciones de datos
    Given que existe un ejemplar con los siguientes datos iniciales:
        | Campo            | Valor                     |
        | Descripcion      | Ejemplar inicial          |
        | Observaciones    | Observaciones iniciales   |
        | FechaAdquisicion | 2024-01-01                |
        | Disponible       | Disponible                |
    And estoy en la página de edición de ese ejemplar
    When actualizo el formulario de ejemplar con los siguientes datos:
        | Campo            | Valor                 |
        | Descripcion      | <Descripcion>         |
        | Observaciones    | <Observaciones>       |
        | FechaAdquisicion | <FechaAdquisicion>    |
        | Disponible       | <Disponible>          |
    And envío el formulario de actualización de ejemplar
    Then debería ver el resultado de actualización de ejemplar "<ResultadoEsperado>"

Examples:
    | Caso | Descripcion                                                                                                                                                                                                        | Observaciones                                                                                              | FechaAdquisicion | Disponible    | ResultadoEsperado |
    | UEJ1 | Ejemplar en buen estado                                                                                                                                                                                            |                                                                                                            | 2025-10-19       | No Disponible | Aceptado          |
    | UEJ2 | Ejemplar en buen estado                                                                                                                                                                                            | Necesita reparación menor                                                                                  | 2025-10-19       | Disponible    | Aceptado          |
    | UEJ3 |                                                                                                                                                                                                                    | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA | 2026-03-23       | Disponible    | Rechazado         |
    | UEJ4 | ABC                                                                                                                                                                                                                | Necesita reparación menor                                                                                  | 2025-10-19       | Disponible    | Aceptado          |
    | UEJ5 | AB                                                                                                                                                                                                                 |                                                                                                            | 2025-10-19       | Disponible    | Rechazado         |
    | UEJ6 | AB                                                                                                                                                                                                                 | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA | 2023-01-15       | No Disponible | Rechazado         |
    | UEJ7 | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA | Necesita reparación menor                                                                                  | 2023-01-15       | Disponible    | Rechazado         |
    | UEJ8 | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA |                                                                                                            | 2026-03-23       | No Disponible | Rechazado         |
    | UEJ9 | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA | 2025-10-19       | Disponible    | Rechazado         |
