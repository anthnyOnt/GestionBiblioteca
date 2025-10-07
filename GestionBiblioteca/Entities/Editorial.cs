using System;
using System.Collections.Generic;

namespace GestionBiblioteca.Entities;

public partial class Editorial
{
    public int Id { get; set; }

    public string? Nombre { get; set; }
    
    public string? Telefono { get; set; }

    public string? Correo { get; set; }

    public string? SitioWeb { get; set; }

    public virtual ICollection<Libro> Libros { get; set; } = new List<Libro>();
}
