Feature: Consultar Información de Libro

Como bibliotecario
Quiero consultar la información de un libro registrado en el sistema
Para verificar sus datos y disponibilidad

  Background:
    Given que soy un usuario autorizado del sistema para libros

  Scenario: Consultar libro existente por ISBN
    Given que existe un libro con ISBN "9781234567890", título "El Principito" y idioma "Español"
    When consulto la información del libro con ISBN "9781234567890"
    Then el sistema muestra la información completa del libro:
        | Campo  | Valor                |
        | ISBN   | 9781234567890        |
        | Titulo | El Principito        |
        | Idioma | Español              |
        | Estado | Activo               |