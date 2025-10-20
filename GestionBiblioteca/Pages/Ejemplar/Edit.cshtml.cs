using System;
using System.Linq;
using System.Threading.Tasks;
using GestionBiblioteca.Services.Ejemplar;
using GestionBiblioteca.Services.Libro;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GestionBiblioteca.Pages.Ejemplar;

public class Edit : PageModel
{
    private readonly IEjemplarService _svc;
    private readonly ILibroService _libroSvc;
    public Edit(IEjemplarService svc, ILibroService libroSvc) { _svc = svc; _libroSvc = libroSvc; }

    [FromRoute] public int Id { get; set; }
    [BindProperty] public Entities.Ejemplar Input { get; set; } = new();
    public SelectList LibrosSelect { get; set; } = new SelectList(Enumerable.Empty<SelectListItem>());

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var e = await _svc.ObtenerSeleccionados(new System.Collections.Generic.List<int> { id });
        var ejemplar = e.FirstOrDefault();
        if (ejemplar == null) return RedirectToPage("Index");
        Input = ejemplar;

        var libros = await _libroSvc.ObtenerTodos();
        LibrosSelect = new SelectList(libros, "Id", "Titulo", Input.IdLibro);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Input.FechaAdquisicion > DateTime.Now)
        {
            ModelState.AddModelError("Input.FechaAdquisicion", "La fecha de adquisición no puede ser futura");
        }

        if (Input.FechaAdquisicion.Year < 1900)
        {
            ModelState.AddModelError("Input.FechaAdquisicion", "La fecha de adquisición debe ser posterior al año 1900");
        }

        if (!ModelState.IsValid)
        {
            var libros = await _libroSvc.ObtenerTodos();
            LibrosSelect = new SelectList(libros, "Id", "Titulo", Input.IdLibro);
            return Page();
        }

        if (string.IsNullOrWhiteSpace(Input.Observaciones))
            Input.Observaciones = "Sin observaciones";

        Input.UltimaActualizacion = DateTime.Now;
        await _svc.Actualizar(Input);
        return RedirectToPage("Index");
    }
}
