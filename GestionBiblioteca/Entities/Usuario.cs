using System;
using System.Collections.Generic;

namespace GestionBiblioteca.Entities;

public partial class Usuario
{
    public int Id { get; set; }

    public string? PrimerNombre { get; set; }

    public string? SegundoNombre { get; set; }

    public string? PrimerApellido { get; set; }

    public string? SegundoApellido { get; set; }

    public string? Ci { get; set; }

    public string? Telefono { get; set; }

    public string? Correo { get; set; }

    public string? Contrasenia { get; set; }

    public string? Rol { get; set; }

    public int? CreadoPor { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? UltimaActualizacion { get; set; }

    public int? Activo { get; set; }

    public virtual ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
}
