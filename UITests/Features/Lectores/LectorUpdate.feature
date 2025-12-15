Feature: Actualización de Lectores a través de la UI
    Como usuario del sistema
    Quiero actualizar información de lectores existentes a través de la interfaz de usuario
    Para poder mantener actualizada la información de los usuarios de la biblioteca

Background:
    Given que existe un lector con los siguientes datos iniciales:
        | Campo            | Valor              |
        | CI               | 1234567            |
        | PrimerNombre     | Test               |
        | SegundoNombre    | Usuario            |
        | PrimerApellido   | Prueba             |
        | SegundoApellido  | Sistema            |
        | Telefono         | 12345678           |
        | Correo           | test@example.com   |
    And estoy en la página de edición de ese lector

Scenario Outline: Actualizar lector con diferentes combinaciones de datos
    When actualizo el formulario de lector con los siguientes datos:
        | Campo            | Valor              |
        | CI               | <CI>               |
        | PrimerNombre     | <PrimerNombre>     |
        | SegundoNombre    | <SegundoNombre>    |
        | PrimerApellido   | <PrimerApellido>   |
        | SegundoApellido  | <SegundoApellido>  |
        | Telefono         | <Telefono>         |
        | Correo           | <Correo>           |
    And envío el formulario de actualización
    Then debería ver el resultado de actualización "<ResultadoEsperado>"

Examples:
    | CasoPrueba | PrimerNombre              | SegundoNombre             | PrimerApellido            | SegundoApellido           | CI          | Telefono     | Correo                                    | ResultadoEsperado |
    | UPD1       | Carlos                    | Manuel                    | Lopez                     | Rodriguez                 | 9876543210  | 87654321     | carlos.lopez@mail.com                     | Aceptado          |
    | UPD2       | Maria                     | Jose                      | Gonzalez                  | Martinez                  | 123456      | 1234567890   | maria@test.com                            | Aceptado          |
    | UPD3       | Pedro                     | Luis                      | Ramirez                   | Fernandez                 | 7654321     | ABC12345     | pedro.ramirez@example.com                 | Rechazado         |
    | UPD4       | Laura                     | AAAAAAAAAAAAAAAAAAAAAAA   | Morales                   | Diaz                      |             | 98765432     | laura@domain.com                          | Rechazado         |
    | UPD5       | Roberto                   | J                         | J                         | Vargas                    | 11223344556 | 11223344     | invalid-email                             | Rechazado         |
    | UPD6       | Sofia                     | Cristina                  | AAAAAAAAAAAAAAAAAAAAAAAA  | J                         | 5544332211  | 55443322     | sofia.cristina@test.co                    | Rechazado         |
    | UPD7       | AAAAAAAAAAAAAAAAAAAAAAAA  | Ana                       | Torres                    | AAAAAAAAAAAAAAAAAAAAAAA   | 998877      | 9988776655   | a@b.c                                     | Aceptado          |
    | UPD8       | Miguel                    | Angel                     | Herrera                   | Castro                    | 445566      | 44556677     | AAAAAAAAAAAAAAAAAAAAAAAAAAAAA@mail.com    | Aceptado          |
    | UPD9       | J                         | AAAAAAAAAAAAAAAAAAAAAAA   | Medina                    | Rojas                     | 5566778899  | 5566778      | miguel@company.org                        | Rechazado         |
    | UPD10      | Gabriela                  | Isabel                    | Ortiz                     | Delgado                   | 667788      | 66778899     | a@b                                       | Rechazado         |
    | UPD11      | Fernando                  | Jose                      | Navarro                   | Ponce                     | 7788990011  | XYZ123@#     | fernando.navarro@enterprise.com           | Rechazado         |
    | UPD12      | J                         | Maria                     | J                         | Silva                     | 8899001122  | 88990011     | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA@mail.com | Rechazado         |
    | UPD13      | Valentina                 | Rosa                      | Campos                    | Reyes                     |             | 99001122     | valentina@system.net                      | Rechazado         |
    | UPD14      | AAAAAAAAAAAAAAAAAAAAAAAA  | Victoria                  | Mendez                    | Paredes                   | 112233      | 10111213     | v@test.io                                 | Aceptado         |
    | UPD15      | Diego                     | AAAAAAAAAAAAAAAAAAAAAAA   | AAAAAAAAAAAAAAAAAAAAAAAA  | Aguirre                   | 2233445566  | 22334455     | noemailformat                             | Rechazado         |
