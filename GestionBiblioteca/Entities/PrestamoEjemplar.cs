using System;
using System.Collections.Generic;

namespace GestionBiblioteca.Entities;

public partial class PrestamoEjemplar
{
    public int IdPrestamo { get; set; }

    public int IdEjemplar { get; set; }

    public DateTime? FechaLimite { get; set; }

    public DateTime? FechaDevolucion { get; set; }

    public bool? Devuelto { get; set; }

    public DateTime? UltimaActualizacion { get; set; }

    public int? Activo { get; set; }

    public virtual Ejemplar IdEjemplarNavigation { get; set; } = null!;

    public virtual Prestamo IdPrestamoNavigation { get; set; } = null!;
}
