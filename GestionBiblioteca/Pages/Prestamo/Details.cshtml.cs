using System.Threading.Tasks;
using GestionBiblioteca.Services.Prestamo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GestionBiblioteca.Pages.Prestamo;

public class Details : PageModel
{
    private readonly IPrestamoService _service;
    public Details(IPrestamoService service) { _service = service; }

    public Entities.Prestamo? Prestamo { get; private set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null) return NotFound();
        var p = await _service.ObtenerPorId(id.Value);
        if (p is null) return NotFound();
        Prestamo = p;
        return Page();
    }
}
