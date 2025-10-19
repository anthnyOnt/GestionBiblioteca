
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GestionBiblioteca.Entities;

public partial class Ejemplar
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un libro")]
    [Display(Name = "Libro")]
    public int? IdLibro { get; set; }

    [Display(Name = "Disponible")]
    public bool? Disponible { get; set; }
    
    [Required(ErrorMessage = "La descripción es obligatoria")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "La descripción debe contener entre 3 y 200 caracteres")]
    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }
    
    [StringLength(300, ErrorMessage = "Las observaciones no pueden superar 300 caracteres")]
    [Display(Name = "Observaciones")]
    public string? Observaciones { get; set; }
    
    [Required(ErrorMessage = "La fecha de adquisición es obligatoria")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de adquisición")]
    public DateTime FechaAdquisicion { get; set; }

    public int? CreadoPor { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? UltimaActualizacion { get; set; }

    public int? Activo { get; set; }

    public virtual ICollection<Estado> Estados { get; set; } = new List<Estado>();

    public virtual Libro? IdLibroNavigation { get; set; }

    public virtual ICollection<PrestamoEjemplar> PrestamoEjemplares { get; set; } = new List<PrestamoEjemplar>();
}
