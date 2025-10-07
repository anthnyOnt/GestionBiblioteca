using System;
using System.Collections.Generic;

namespace GestionBiblioteca.Entities;

public partial class Autor
{
    public int Id { get; set; }

    public string? Nombre { get; set; }

    public string? Apellido { get; set; }

    public virtual ICollection<Libro> IdLibros { get; set; } = new List<Libro>();
}
