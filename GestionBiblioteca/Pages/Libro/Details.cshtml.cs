using System.Threading.Tasks;
using GestionBiblioteca.Services.Libro;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GestionBiblioteca.Pages.Libro;

public class Details : PageModel
{
    private readonly ILibroService _svc;
    public Details(ILibroService svc) { _svc = svc; }

    public Entities.Libro? Input { get; set; }

    public async Task OnGetAsync(int id)
    {
        Input = await _svc.ObtenerPorId(id);
    }
}
