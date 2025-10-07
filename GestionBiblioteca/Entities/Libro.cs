using System;
using System.Collections.Generic;

namespace GestionBiblioteca.Entities;

public partial class Libro
{
    public int Id { get; set; }

    public int? IdEditorial { get; set; }

    public string? Titulo { get; set; }

    public string? Isbn { get; set; }

    public string? Sinopsis { get; set; }

    public DateTime? FechaPublicacion { get; set; }

    public string? Idioma { get; set; }

    public string? Edicion { get; set; }

    public virtual ICollection<Ejemplar> Ejemplares { get; set; } = new List<Ejemplar>();

    public virtual Editorial? IdEditorialNavigation { get; set; }

    public virtual ICollection<Autor> IdAutores { get; set; } = new List<Autor>();

    public virtual ICollection<Categoria> IdCategoria { get; set; } = new List<Categoria>();
}
