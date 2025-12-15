Feature: Creación de Libros a través de la UI
    Como usuario del sistema
    Quiero crear nuevos libros a través de la interfaz de usuario
    Para poder gestionar el catálogo de la biblioteca

Background:
    Given que estoy en la página de creación de libros

Scenario Outline: Crear libro con diferentes combinaciones de datos
    When lleno el formulario de libro con los siguientes datos:
        | Campo            | Valor              |
        | Titulo           | <Titulo>           |
        | ISBN             | <ISBN>             |
        | Sinopsis         | <Sinopsis>         |
        | FechaPub         | <FechaPub>         |
        | Idioma           | <Idioma>           |
        | Edicion          | <Edicion>          |
    And envío el formulario de libro
    Then debería ver el resultado del libro "<ResultadoEsperado>"

Examples:
    | Caso  | Titulo                             | ISBN              | Sinopsis                | FechaPub   | Idioma        | Edicion       | ResultadoEsperado |
    | LIB1  | Cien años de Soledad               | 12345678901234    |                         | 01/01/2026 | AAAAAAAAAAAAAAAAAAA | AAAAAAAAAAAAAAAAAAA | Rechazado         |
    | LIB2  | Cien años de Soledad               | 0-306-10615       | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA | 01/15/2020 | A             | AAAAAAAAAAAAAAAAAAAAA | Rechazado         |
    | LIB3  | Cien años de Soledad               | 0-306-20615       |                         | 01/15/2020 | English       | AAAAAAAAAAAAAAAAAAA | Aceptado          |
    | LIB4  | A                                  |                   |                         | 01/15/2020 | AAAAAAAAAAAAAAAAAAAAA |                   | Rechazado         |
    | LIB5  | A                                  | 12345678901234    | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA | 10/19/2025 | Español13@#   | AAAAAAAAAAAAAAAAAAA | Rechazado         |
    | LIB6  | A                                  |                   | Una novela sobre la vida de una familia a lo largo de varias generaciones en el pueblo ficticio de Macondo. | 01/15/2020 | English       |                   | Aceptado          |
    | LIB7  | A                                  | 12345678901234    |                         | 01/15/2020 | Es            | AAAAAAAAAAAAAAAAAAA | Rechazado         |
    | LIB8  | A                                  | 0-306-3           | Una novela sobre la vida de una familia a lo largo de varias generaciones en el pueblo ficticio de Macondo. | 10/19/2025 | AAAAAAAAAAAAAAAAAAA | AAAAAAAAAAAAAAAAAAAAA | Rechazado         |
    | LIB9  | A                                  | 0-306-40615       | Una novela sobre la vida de una familia a lo largo de varias generaciones en el pueblo ficticio de Macondo. | 01/01/2026 | A             | 2da edición   | Rechazado         |
    | LIB10 | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA | 12345678901234    | Una novela sobre la vida de una familia a lo largo de varias generaciones en el pueblo ficticio de Macondo. | 01/15/2020 | Es            | AAAAAAAAAAAAAAAAAAAAA | Rechazado         |
    | LIB11 | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA | 0-306-50615       | Una novela sobre la vida de una familia a lo largo de varias generaciones en el pueblo ficticio de Macondo. | 01/15/2020 | AAAAAAAAAAAAAAAAAAA | 2da edición   | Aceptado          |
    | LIB12 | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA |                   |                         | 10/19/2025 | A             |                   | Rechazado         |
    | LIB13 | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA | 12345678901234    | Una novela sobre la vida de una familia a lo largo de varias generaciones en el pueblo ficticio de Macondo. | 01/01/2026 | AAAAAAAAAAAAAAAAAAAAA | AAAAAAAAAAAAAAAAAAA | Rechazado         |
    | LIB14 | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA | 0-306-60615       | Una novela sobre la vida de una familia a lo largo de varias generaciones en el pueblo ficticio de Macondo. | 01/15/2020 | Español13@#   | 2da edición   | Rechazado         |
    | LIB15 | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA |                   | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA | 01/01/2026 | English       | AAAAAAAAAAAAAAAAAAA | Rechazado         |
    | LIB16 | ¿Quién mató a Palomino Montero?   | 0-306-7615        |                         | 01/01/2026 | Español13@#   |                   | Rechazado         |
    | LIB17 | ¿Quién mató a Palomino Montero?   | 12345678901234    | Una novela sobre la vida de una familia a lo largo de varias generaciones en el pueblo ficticio de Macondo. | 10/19/2025 | English       | AAAAAAAAAAAAAAAAAAAAA | Rechazado         |
    | LIB18 | ¿Quién mató a Palomino Montero?   | 0-306-80615       |                         | 01/01/2026 | Es            | 2da edición   | Rechazado         |
    | LIB19 | ¿Quién mató a Palomino Montero?   | 0-306-90615       | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA | 01/15/2020 | AAAAAAAAAAAAAAAAAAA |                   | Rechazado         |
    | LIB20 | ¿Quién mató a Palomino Montero?   |                   | Una novela sobre la vida de una familia a lo largo de varias generaciones en el pueblo ficticio de Macondo. | 01/15/2020 | A             | AAAAAAAAAAAAAAAAAAA | Rechazado         |
    | LIB21 | ¿Quién mató a Palomino Montero?   | 12345678901234    | Una novela sobre la vida de una familia a lo largo de varias generaciones en el pueblo ficticio de Macondo. | 10/19/2025 | AAAAAAAAAAAAAAAAAAAAA | 2da edición   | Rechazado         |
    | LIB22 |                                    |                   | Una novela sobre la vida de una familia a lo largo de varias generaciones en el pueblo ficticio de Macondo. | 01/01/2026 | AAAAAAAAAAAAAAAAAAA | AAAAAAAAAAAAAAAAAAA | Rechazado         |
    | LIB23 |                                    | 12345678901234    |                         | 01/15/2020 | A             | 2da edición   | Rechazado         |
    | LIB24 |                                    | 0-306-10615       | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA | 01/15/2020 | AAAAAAAAAAAAAAAAAAAAA |                   | Rechazado         |
    | LIB25 |                                    | 0-306-11615       | Una novela sobre la vida de una familia a lo largo de varias generaciones en el pueblo ficticio de Macondo. | 10/19/2025 | Español13@#   | AAAAAAAAAAAAAAAAAAA | Rechazado         |
    | LIB26 |                                    |                   | Una novela sobre la vida de una familia a lo largo de varias generaciones en el pueblo ficticio de Macondo. | 01/01/2026 | English       | AAAAAAAAAAAAAAAAAAAAA | Rechazado         |
    | LIB27 |                                    | 0-306-12615       | Una novela sobre la vida de una familia a lo largo de varias generaciones en el pueblo ficticio de Macondo. | 10/19/2025 | Es            |                   | Rechazado         |
    | LIB28 | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA | 12345678901234    | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA | 10/19/2025 | English       | 2da edición   | Rechazado         |
    | LIB29 | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA | 0-306-13615       | Una novela sobre la vida de una familia a lo largo de varias generaciones en el pueblo ficticio de Macondo. | 01/01/2026 | English       |                   | Rechazado         |
    | LIB30 | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA | 0-306-14615       | Una novela sobre la vida de una familia a lo largo de varias generaciones en el pueblo ficticio de Macondo. | 01/15/2020 | Es            | AAAAAAAAAAAAAAAAAAA | Rechazado         |
    | LIB31 | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA |                   |                         | 10/19/2025 | AAAAAAAAAAAAAAAAAAA | 2da edición   | Rechazado         |
    | LIB32 | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA | 12345678901234    | Una novela sobre la vida de una familia a lo largo de varias generaciones en el pueblo ficticio de Macondo. | 01/01/2026 | A             |                   | Rechazado         |
    | LIB33 | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA | 0-306-15615       | Una novela sobre la vida de una familia a lo largo de varias generaciones en el pueblo ficticio de Macondo. | 01/15/2020 | AAAAAAAAAAAAAAAAAAAAA | AAAAAAAAAAAAAAAAAAA | Rechazado         |
    | LIB34 | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA |                   |                         | 01/15/2020 | Español13@#   | AAAAAAAAAAAAAAAAAAAAA | Rechazado         |
    | LIB35 | Cien años de Soledad               | 0-306-16615       | Una novela sobre la vida de una familia a lo largo de varias generaciones en el pueblo ficticio de Macondo. | 10/19/2025 | A             | AAAAAAAAAAAAAAAAAAA | Rechazado         |
    | LIB36 | Cien años de Soledad               | 0-306-17615       |                         | 01/01/2026 | AAAAAAAAAAAAAAAAAAAAA | AAAAAAAAAAAAAAAAAAAAA | Rechazado         |
    | LIB37 | Cien años de Soledad               |                   | Una novela sobre la vida de una familia a lo largo de varias generaciones en el pueblo ficticio de Macondo. | 01/15/2020 | Español13@#   | 2da edición   | Rechazado         |
    | LIB38 | Cien años de Soledad               | 12345678901234    | Una novela sobre la vida de una familia a lo largo de varias generaciones en el pueblo ficticio de Macondo. | 01/15/2020 | English       |                   | Rechazado         |
    | LIB39 | Cien años de Soledad               | 0-306-18615       |                         | 10/19/2025 | English       | AAAAAAAAAAAAAAAAAAA | Rechazado         |
    | LIB40 | Cien años de Soledad               |                   | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA | 01/01/2026 | Es            | 2da edición   | Rechazado         |
    | LIB41 | Cien años de Soledad               | 12345678901234    | Una novela sobre la vida de una familia a lo largo de varias generaciones en el pueblo ficticio de Macondo. | 01/15/2020 | AAAAAAAAAAAAAAAAAAA |                   | Rechazado         |
