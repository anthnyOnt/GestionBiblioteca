using System;
using System.Linq;
using System.Threading.Tasks;
using GestionBiblioteca.Services.Ejemplar;
using GestionBiblioteca.Services.Libro;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GestionBiblioteca.Pages.Ejemplar;

public class Create : PageModel
{
    private readonly IEjemplarService _svc;
    private readonly ILibroService _libroSvc;
    public Create(IEjemplarService svc, ILibroService libroSvc) { _svc = svc; _libroSvc = libroSvc; }

    [BindProperty] public Entities.Ejemplar Input { get; set; } = new();
    public SelectList LibrosSelect { get; set; } = new SelectList(Enumerable.Empty<SelectListItem>());

    public async Task OnGetAsync()
    {
        var libros = await _libroSvc.ObtenerTodos();
        LibrosSelect = new SelectList(libros, "Id", "Titulo");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Custom validation for FechaAdquisicion
        if (Input.FechaAdquisicion > DateTime.Now)
        {
            ModelState.AddModelError("Input.FechaAdquisicion", "La fecha de adquisición no puede ser futura");
        }

        // Validate that the date is reasonable (not too old, e.g., before year 1900)
        if (Input.FechaAdquisicion.Year < 1900)
        {
            ModelState.AddModelError("Input.FechaAdquisicion", "La fecha de adquisición debe ser posterior al año 1900");
        }

        if (!ModelState.IsValid) {
            var libros = await _libroSvc.ObtenerTodos();
            LibrosSelect = new SelectList(libros, "Id", "Titulo");
            return Page();
        }

        if (string.IsNullOrWhiteSpace(Input.Observaciones))
            Input.Observaciones = "Sin observaciones";

        Input.FechaCreacion = DateTime.Now;
        Input.CreadoPor = 1; // will be replaced by service's ObtenerIdSesion
        Input.Activo = 1;

        await _svc.Crear(Input);
        return RedirectToPage("Index");
    }
}
