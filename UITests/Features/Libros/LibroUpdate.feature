Feature: Actualización de Libros a través de la UI
    Como usuario del sistema
    Quiero actualizar información de libros existentes a través de la interfaz de usuario
    Para poder mantener actualizado el catálogo de la biblioteca

Background:
    Given que existe un libro con los siguientes datos iniciales:
        | Campo            | Valor                |
        | Titulo           | Libro de Prueba      |
        | ISBN             | TEST-INITIAL         |
        | Sinopsis         | Sinopsis inicial     |
        | FechaPub         | 01/01/2020           |
        | Idioma           | Español              |
        | Edicion          | Primera              |
    And estoy en la página de edición de ese libro

Scenario Outline: Actualizar libro con diferentes combinaciones de datos
    When actualizo el formulario de libro con los siguientes datos:
        | Campo            | Valor          |
        | Titulo           | <Titulo>       |
        | ISBN             | <ISBN>         |
        | Sinopsis         | <Sinopsis>     |
        | FechaPub         | <FechaPub>     |
        | Idioma           | <Idioma>       |
        | Edicion          | <Edicion>      |
    And envío el formulario de actualización de libro
    Then debería ver el resultado de actualización de libro "<ResultadoEsperado>"

Examples:
    | Caso  | Titulo                                              | ISBN              | Sinopsis                                                                                                                                                                                                 | FechaPub   | Idioma              | Edicion                | ResultadoEsperado |
    | ULB1  | El Quijote                                          | 1234567890123     | Una obra maestra                                                                                                                                                                                         | 01/15/2020 | Español             | Segunda                | Aceptado          |
    | ULB2  |                                                     | 9876543210987     | Sinopsis válida                                                                                                                                                                                          | 06/10/2021 | English             | Primera                | Rechazado         |
    | ULB3  | Cien Años de Soledad                                | 12345678901234    | Texto corto                                                                                                                                                                                              | 12/25/2019 | Francés             | Tercera                | Rechazado         |
    | ULB4  | A                                                   |                   |                                                                                                                                                                                                          |            |                     |                        | Aceptado          |
    | ULB5  | La Odisea                                           | 111               | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA | 01/01/2026 | A                   | AAAAAAAAAAAAAAAAAAAAA  | Rechazado         |
    | ULB6  | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA | 0-306-40615-2     | Sinopsis de prueba                                                                                                                                                                                       | 03/20/2022 | AAAAAAAAAAAAAAAAAAA | A                      | Rechazado         |
    | ULB7  | El Principito                                       |                   | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA | 01/01/2026 | Alemán              | AAAAAAAAAAAAAAAAAAAAA  | Rechazado         |
    | ULB8  | Hamlet                                              | 1111111111111     | Texto de ejemplo                                                                                                                                                                                         | 11/11/2017 | A                   | Quinta                 | Rechazado         |
    | ULB9 | Moby Dick                                           | 999-888-777       | Una historia épica                                                                                                                                                                                       | 08/15/2023 | AAAAAAAAAAAAAAAAAAA | Segunda                | Rechazado         |
    | ULB10 | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA | 5555555555555     |                                                                                                                                                                                                          | 01/01/2026 | Portugués           | A                      | Rechazado         |
    | ULB11 | La Ilíada                                           | ABC-DEF-GHI       | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA | 02/28/2021 | A                   | Primera                | Rechazado         |
    | ULB12 | Don Juan Tenorio                                    |                   | Sinopsis breve                                                                                                                                                                                           | 05/05/2019 | Catalán             | AAAAAAAAAAAAAAAAAAAAA  | Rechazado         |
    | ULB13 | Fausto                                              | 2468101214161     | Texto medio                                                                                                                                                                                              | 01/01/2026 | AAAAAAAAAAAAAAAAAAA | Sexta                  | Rechazado         |
    | ULB14 | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA | 1357924680246     | Una sinopsis                                                                                                                                                                                             | 09/30/2020 | A                   | AAAAAAAAAAAAAAAAAAAAA  | Rechazado         |
    | ULB15 | Guerra y Paz                                        | 9999999999999     | Sinopsis extensa pero válida                                                                                                                                                                             | 01/01/2026 | Ruso                | A                      | Rechazado         |
    | ULB16 | Los Miserables                                      | 0000000000000     | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA | 12/31/2024 | AAAAAAAAAAAAAAAAAAA | AAAAAAAAAAAAAAAAAAAAA  | Rechazado         |
