using System.Threading.Tasks;
using GestionBiblioteca.Services.Ejemplar;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GestionBiblioteca.Pages.Ejemplar;

public class Details : PageModel
{
    private readonly IEjemplarService _svc;
    public Details(IEjemplarService svc) { _svc = svc; }

    public Entities.Ejemplar? Input { get; set; }

    public async Task OnGetAsync(int id)
    {
        var list = await _svc.ObtenerSeleccionados(new System.Collections.Generic.List<int> { id });
        Input = list.FirstOrDefault();
    }
}
