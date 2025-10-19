using System.Threading.Tasks;
using GestionBiblioteca.Services.Ejemplar;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GestionBiblioteca.Pages.Ejemplar;

public class Delete : PageModel
{
    private readonly IEjemplarService _svc;
    public Delete(IEjemplarService svc) { _svc = svc; }

    [BindProperty] public Entities.Ejemplar? Input { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var list = await _svc.ObtenerSeleccionados(new System.Collections.Generic.List<int> { id });
        Input = list.FirstOrDefault();
        if (Input == null) return RedirectToPage("Index");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        await _svc.Eliminar(id);
        return RedirectToPage("Index");
    }
}
