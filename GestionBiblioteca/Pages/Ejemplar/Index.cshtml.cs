using System.Collections.Generic;
using System.Threading.Tasks;
using GestionBiblioteca.Services.Ejemplar;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GestionBiblioteca.Pages.Ejemplar;

public class Index : PageModel
{
    private readonly IEjemplarService _svc;
    public Index(IEjemplarService svc) { _svc = svc; }

    public IList<Entities.Ejemplar> Ejemplares { get; private set; } = new List<Entities.Ejemplar>();

    public async Task OnGetAsync()
    {
        Ejemplares = await _svc.ObtenerDisponibles();
    }
}
