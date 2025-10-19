using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GestionBiblioteca.Entities;

public partial class Libro
{
    public int Id { get; set; }

    [Display(Name = "Editorial")]
    public int? IdEditorial { get; set; }

    [Required(ErrorMessage = "El título es obligatorio")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "El título debe contener entre 1 y 200 caracteres")]
    [Display(Name = "Título")]
    public string? Titulo { get; set; }

    [RegularExpression(@"^(?:ISBN(?:-1[03])?:?\s*)?(?=[0-9X]{10}$|(?=(?:[0-9]+[-\s]){3})[-\s0-9X]{13}$|97[89][0-9]{10}$|(?=(?:[0-9]+[-\s]){4})[-\s0-9]{17}$)(?:97[89][-\s]?)?[0-9]{1,5}[-\s]?[0-9]+[-\s]?[0-9]+[-\s]?[0-9X]$", 
        ErrorMessage = "El formato del ISBN no es válido. Debe ser ISBN-10 o ISBN-13")]
    [StringLength(20, ErrorMessage = "El ISBN no puede superar 20 caracteres")]
    [Display(Name = "ISBN")]
    public string? Isbn { get; set; }

    [StringLength(1000, ErrorMessage = "La sinopsis no puede superar 1000 caracteres")]
    [Display(Name = "Sinopsis")]
    public string? Sinopsis { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Fecha de publicación")]
    public DateTime? FechaPublicacion { get; set; }

    [StringLength(50, MinimumLength = 2, ErrorMessage = "El idioma debe contener entre 2 y 50 caracteres")]
    [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El idioma solo puede contener letras")]
    [Display(Name = "Idioma")]
    public string? Idioma { get; set; }

    [StringLength(50, ErrorMessage = "La edición no puede superar 50 caracteres")]
    [Display(Name = "Edición")]
    public string? Edicion { get; set; }

    public virtual ICollection<Ejemplar> Ejemplares { get; set; } = new List<Ejemplar>();

    public virtual Editorial? IdEditorialNavigation { get; set; }

    public virtual ICollection<Autor> IdAutores { get; set; } = new List<Autor>();

    public virtual ICollection<Categoria> IdCategoria { get; set; } = new List<Categoria>();
    
    public int? CreadoPor { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? UltimaActualizacion { get; set; }

    public int? Activo { get; set; }
}
