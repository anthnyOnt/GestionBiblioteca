using System;
using System.Collections.Generic;

namespace GestionBiblioteca.Entities;

public partial class Categoria
{
    public sbyte Id { get; set; }

    public string? Nombre { get; set; }

    public int? CreadoPor { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? UltimaActualizacion { get; set; }

    public int? Activo { get; set; }

    public virtual ICollection<Libro> IdLibros { get; set; } = new List<Libro>();
}
