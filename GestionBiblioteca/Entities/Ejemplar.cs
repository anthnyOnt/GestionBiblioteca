using System;
using System.Collections.Generic;

namespace GestionBiblioteca.Entities;

public partial class Ejemplar
{
    public int Id { get; set; }

    public int? IdLibro { get; set; }

    public bool? Disponible { get; set; }

    public int? CreadoPor { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? UltimaActualizacion { get; set; }

    public int? Activo { get; set; }

    public virtual ICollection<Estado> Estados { get; set; } = new List<Estado>();

    public virtual Libro? IdLibroNavigation { get; set; }

    public virtual ICollection<PrestamoEjemplar> PrestamoEjemplares { get; set; } = new List<PrestamoEjemplar>();
}
