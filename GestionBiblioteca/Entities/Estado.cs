using System;
using System.Collections.Generic;

namespace GestionBiblioteca.Entities;

public partial class Estado
{
    public int Id { get; set; }

    public int? IdEjemplar { get; set; }

    public string? Observacion { get; set; }

    public DateTime? FechaDescripcion { get; set; }

    public int? CreadoPor { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? UltimaActualizacion { get; set; }

    public int? Activo { get; set; }

    public virtual Ejemplar? IdEjemplarNavigation { get; set; }
}
