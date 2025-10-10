using System.Collections.Generic;
using System.Threading.Tasks;
using GestionBiblioteca.Services.Prestamo;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GestionBiblioteca.Pages.Prestamo;

public class Index : PageModel
{
    private readonly IPrestamoService _service;
    public Index(IPrestamoService service) { _service = service; }

    public IList<Entities.Prestamo> Prestamos { get; private set; } = new List<Entities.Prestamo>();

    public async Task OnGetAsync() => Prestamos = await _service.ObtenerTodos();
}
