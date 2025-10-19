using System.Collections.Generic;
using System.Threading.Tasks;
using GestionBiblioteca.Services.Libro;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GestionBiblioteca.Pages.Libro;

public class Index : PageModel
{
    private readonly ILibroService _svc;
    public Index(ILibroService svc) { _svc = svc; }

    public IList<Entities.Libro> Libros { get; private set; } = new List<Entities.Libro>();

    public async Task OnGetAsync()
    {
        Libros = await _svc.ObtenerTodos();
    }
}
