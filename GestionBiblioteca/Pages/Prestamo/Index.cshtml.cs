using GestionBiblioteca.Services.Prestamo;
using GestionBiblioteca.Services.Usuario;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GestionBiblioteca.Pages.Prestamo;

public class Index : PageModel
{
    private readonly IPrestamoService _service;

    public Index(IPrestamoService service)
    {
        _service = service;
    }
    
    public IList<Entities.Prestamo> Prestamos { get;set; } = default!;

    public async Task OnGetAsync()
    {
        Prestamos = await _service.ObtenerTodos();
    }
}

