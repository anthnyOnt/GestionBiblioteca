Feature: Creación de Lectores a través de la UI
    Como usuario del sistema
    Quiero crear nuevos lectores a través de la interfaz de usuario
    Para poder gestionar los usuarios de la biblioteca

Background:
    Given que estoy en la página de creación de lectores

Scenario Outline: Crear lector con diferentes combinaciones de datos
    When lleno el formulario de lector con los siguientes datos:
        | Campo            | Valor              |
        | PrimerNombre     | <PrimerNombre>     |
        | SegundoNombre    | <SegundoNombre>    |
        | PrimerApellido   | <PrimerApellido>   |
        | SegundoApellido  | <SegundoApellido>  |
        | CI               | <CI>               |
        | Telefono         | <Telefono>         |
        | Correo           | <Correo>           |
    And envío el formulario
    Then debería ver el resultado "<ResultadoEsperado>"

Examples:
    | CasoPrueba | PrimerNombre                  | SegundoNombre                 | PrimerApellido                | SegundoApellido               | CI          | Telefono       | Correo                                                 | ResultadoEsperado |
    | LEC1       | Juan                          | Sol                           | Saa                           | Saa                           | 1464324567  | 1234567890     | a@b.c                                                  | Aceptado          |
    | LEC2       | Juan                          | AAAAAAAAAAAAAAAAAAAAAAAAA     | AAAAAAAAAAAAAAAAAAAAAAAAA     | AAAAAAAAAAAAAAAAAAAAAAAAA     | 12345       | 1234567        | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA@mail.com                | Rechazado         |
    | LEC3       | Juan                          | J                             | J                             | J                             | 12345678910 | 123456789101   | a@b                                                    | Rechazado         |
    | LEC4       | Juan                          | AAAAAAAAAAAAAAAAAAAAAAAAAA    | AAAAAAAAAAAAAAAAAAAAAAAAAA    | AAAAAAAAAAAAAAAAAAAAAAAAAA    |             | ABC123@#       | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA@mail.com | Rechazado         |
    | LEC5       | Juan                          | Jose                          | Ontiveros                     | García                        | 9416526     | 12345678       | Estonoesuncorreo                                       | Rechazado         |
    | LEC6       | Ana                           | Sol                           | AAAAAAAAAAAAAAAAAAAAAAAAA     | J                             |             | 12345678       | User@example.com                                       | Rechazado         |
    | LEC7       | Ana                           | AAAAAAAAAAAAAAAAAAAAAAAAA     | J                             | AAAAAAAAAAAAAAAAAAAAAAAAAA    | 9416526     | 12345678       | a@b.c                                                  | Rechazado         |
    | LEC8       | Ana                           | J                             | AAAAAAAAAAAAAAAAAAAAAAAAAA    | García                        | 9416526     | 1234567890     | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA@mail.com                | Rechazado         |
    | LEC9       | Ana                           | AAAAAAAAAAAAAAAAAAAAAAAAAA    | Ontiveros                     | García                        | 1464324567  | 1234567        | a@b                                                    | Rechazado         |
    | LEC10      | Ana                           | Jose                          | Ontiveros                     | Saa                           | 12345       | 123456789101   | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA@mail.com | Rechazado         |
    | LEC11      | Ana                           | Jose                          | Saa                           | AAAAAAAAAAAAAAAAAAAAAAAAA     | 12345678910 | ABC123@#       | Estonoesuncorreo                                       | Rechazado         |
    | LEC12      | AAAAAAAAAAAAAAAAAAAAAAAAA     | AAAAAAAAAAAAAAAAAAAAAAAAA     | AAAAAAAAAAAAAAAAAAAAAAAAAA    | García                        | 12345       | ABC123@#       | User@example.com                                       | Rechazado         |
    | LEC13      | AAAAAAAAAAAAAAAAAAAAAAAAA     | J                             | Ontiveros                     | Saa                           | 12345678910 | 12345678       | a@b.c                                                  | Rechazado         |
    | LEC14      | AAAAAAAAAAAAAAAAAAAAAAAAA     | AAAAAAAAAAAAAAAAAAAAAAAAAA    | Ontiveros                     | AAAAAAAAAAAAAAAAAAAAAAAAA     |             | 12345678       | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA@mail.com                | Rechazado         |
    | LEC15      | AAAAAAAAAAAAAAAAAAAAAAAAA     | Jose                          | Saa                           | J                             | 9416526     | 1234567890     | a@b                                                    | Rechazado         |
    | LEC16      | AAAAAAAAAAAAAAAAAAAAAAAAA     | Jose                          | AAAAAAAAAAAAAAAAAAAAAAAAA     | AAAAAAAAAAAAAAAAAAAAAAAAAA    | 9416526     | 1234567        | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA@mail.com | Rechazado         |
    | LEC17      | AAAAAAAAAAAAAAAAAAAAAAAAA     | Sol                           | J                             | García                        | 1464324567  | 123456789101   | Estonoesuncorreo                                       | Rechazado         |
    | LEC18      | J                             | J                             | Ontiveros                     | J                             | 9416526     | 123456789101   | User@example.com                                       | Rechazado         |
    | LEC19      | J                             | AAAAAAAAAAAAAAAAAAAAAAAAAA    | Saa                           | AAAAAAAAAAAAAAAAAAAAAAAAAA    | 1464324567  | ABC123@#       | a@b.c                                                  | Rechazado         |
    | LEC20      | J                             | Jose                          | AAAAAAAAAAAAAAAAAAAAAAAAA     | García                        | 12345       | 1234567        | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA@mail.com                | Rechazado         |
    | LEC21      | J                             | Jose                          | J                             | García                        | 12345678910 | 12345678       | a@b                                                    | Rechazado         |
    | LEC22      | J                             | Sol                           | AAAAAAAAAAAAAAAAAAAAAAAAAA    | Saa                           |             | 1234567890     | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA@mail.com | Rechazado         |
    | LEC23      | J                             | AAAAAAAAAAAAAAAAAAAAAAAAA     | Ontiveros                     | AAAAAAAAAAAAAAAAAAAAAAAAA     | 9416526     | 1234567        | Estonoesuncorreo                                       | Rechazado         |
    | LEC24      | AAAAAAAAAAAAAAAAAAAAAAAAAA    | AAAAAAAAAAAAAAAAAAAAAAAAAA    | AAAAAAAAAAAAAAAAAAAAAAAAA     | García                        |             | 1234567        | User@example.com                                       | Rechazado         |
    | LEC25      | AAAAAAAAAAAAAAAAAAAAAAAAAA    | Jose                          | J                             | Saa                           | 9416526     | 123456789101   | a@b.c                                                  | Rechazado         |
    | LEC26      | AAAAAAAAAAAAAAAAAAAAAAAAAA    | Jose                          | AAAAAAAAAAAAAAAAAAAAAAAAAA    | AAAAAAAAAAAAAAAAAAAAAAAAA     | 9416526     | ABC123@#       | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA@mail.com                | Rechazado         |
    | LEC27      | AAAAAAAAAAAAAAAAAAAAAAAAAA    | Sol                           | Ontiveros                     | J                             | 1464324567  | 12345678       | a@b                                                    | Rechazado         |
    | LEC28      | AAAAAAAAAAAAAAAAAAAAAAAAAA    | AAAAAAAAAAAAAAAAAAAAAAAAA     | Ontiveros                     | AAAAAAAAAAAAAAAAAAAAAAAAAA    | 12345       | 12345678       | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA@mail.com | Rechazado         |
    | LEC29      | AAAAAAAAAAAAAAAAAAAAAAAAAA    | J                             | Saa                           | García                        | 12345678910 | 1234567890     | Estonoesuncorreo                                       | Rechazado         |
    | LEC30      | Juan                          | Jose                          | AAAAAAAAAAAAAAAAAAAAAAAAAA    | J                             | 12345       | 1234567890     | User@example.com                                       | Rechazado         |
    | LEC31      | Juan                          | Jose                          | Ontiveros                     | AAAAAAAAAAAAAAAAAAAAAAAAAA    | 12345678910 | 1234567        | a@b.c                                                  | Rechazado         |
    | LEC32      | Juan                          | Sol                           | Ontiveros                     | García                        |             | 123456789101   | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA@mail.com                | Rechazado         |
    | LEC33      | Juan                          | AAAAAAAAAAAAAAAAAAAAAAAAA     | Saa                           | García                        | 9416526     | ABC123@#       | a@b                                                    | Rechazado         |
    | LEC34      | Juan                          | J                             | AAAAAAAAAAAAAAAAAAAAAAAAA     | Saa                           | 9416526     | 12345678       | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA@mail.com | Rechazado         |
    | LEC35      | Juan                          | AAAAAAAAAAAAAAAAAAAAAAAAAA    | J                             | AAAAAAAAAAAAAAAAAAAAAAAAA     | 1464324567  | 12345678       | Estonoesuncorreo                                       | Rechazado         |
