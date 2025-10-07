using GestionBiblioteca.Entities;
using GestionBiblioteca.Services.Prestamo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GestionBiblioteca.Pages.Prestamo;

public class Details : PageModel
{
    private readonly IPrestamoService _service;

    public Details(IPrestamoService service)
    {
        _service = service;
    }

    public Entities.Prestamo Prestamo { get; set; } = default!;
    
    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var prestamo = await _service.ObtenerPorId(id.Value);

        if (prestamo is not null)
        {
            Prestamo = prestamo;

            return Page();
        }

        return NotFound();
    }
}