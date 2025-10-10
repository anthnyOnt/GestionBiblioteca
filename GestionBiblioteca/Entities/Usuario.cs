using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GestionBiblioteca.Entities;

public partial class Usuario
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El primer nombre es obligatorio.")]
    [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "Solo se permiten letras.")]
    [StringLength(25, MinimumLength = 3, ErrorMessage = "El primer nombre debe contener entre 3 a 30 caracteres")]
    [Display(Name = "Primer nombre")]
    public string? PrimerNombre { get; set; }

    [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "Solo se permiten letras.")]
    [StringLength(25, MinimumLength = 3, ErrorMessage = "El segundo nombre debe contener entre 3 a 30 caracteres")]
    [Display(Name = "Segundo nombre")]
    public string? SegundoNombre { get; set; }

    [Required(ErrorMessage = "El primer apellido es obligatorio.")]
    [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "Solo se permiten letras.")]
    [StringLength(25, MinimumLength = 3, ErrorMessage = "El primer apellido debe contener entre 3 a 30 caracteres")]
    [Display(Name = "Primer apellido")]
    public string? PrimerApellido { get; set; }

    [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "Solo se permiten letras.")]
    [StringLength(25, MinimumLength = 3, ErrorMessage = "El segundo apellido debe contener entre 3 a 30 caracteres")]
    [Display(Name = "Segundo apellido")]
    public string? SegundoApellido { get; set; }

    [Required(ErrorMessage = "El número de cédula es obligatorio.")]
    [RegularExpression(@"^\d+$", ErrorMessage = "Solo se permiten dígitos.")]
    [StringLength(10, MinimumLength = 6, ErrorMessage = "El número de cédula debe contener entre 6 a 10 digitos")]
    [Display(Name = "Cédula de identidad")]
    public string? Ci { get; set; }

    [Required(ErrorMessage = "El número de teléfono es obligatorio")]
    [Phone(ErrorMessage = "El número de teléfono no es válido.")]
    [RegularExpression(@"^\d+$", ErrorMessage = "Solo se permiten dígitos.")]
    [StringLength(10, ErrorMessage = "El teléfono debe contener entre 8 a 10 caracteres")]
    [Display(Name = "Teléfono")]
    public string? Telefono { get; set; }

    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
    [StringLength(45,MinimumLength = 5, ErrorMessage = "El correo electrónico debe contener entre 5 a 45 caracteres")]
    [Display(Name = "Correo electrónico")]
    public string? Correo { get; set; }

    public string? Contrasenia { get; set; }

    public string? Rol { get; set; }

    public int? CreadoPor { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? UltimaActualizacion { get; set; }

    public int? Activo { get; set; }

    public virtual ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
}
