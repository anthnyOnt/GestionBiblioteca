using System.Threading.Tasks;
using GestionBiblioteca.Services.Libro;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GestionBiblioteca.Pages.Libro;

public class Delete : PageModel
{
    private readonly ILibroService _svc;
    public Delete(ILibroService svc) { _svc = svc; }

    [BindProperty] public Entities.Libro? Input { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Input = await _svc.ObtenerPorId(id);
        if (Input == null) return RedirectToPage("Index");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        await _svc.Eliminar(id);
        return RedirectToPage("Index");
    }
}
