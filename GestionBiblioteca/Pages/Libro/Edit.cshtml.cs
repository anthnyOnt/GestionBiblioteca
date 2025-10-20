using System;
using System.Linq;
using System.Threading.Tasks;
using GestionBiblioteca.Services.Libro;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GestionBiblioteca.Pages.Libro;

public class Edit : PageModel
{
    private readonly ILibroService _svc;
    public Edit(ILibroService svc) { _svc = svc; }

    [FromRoute] public int Id { get; set; }
    [BindProperty] public Entities.Libro Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var l = await _svc.ObtenerPorId(id);
        if (l == null) return RedirectToPage("Index");
        Input = l;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(Input.Titulo))
        {
            ModelState.AddModelError("Input.Titulo", "El título es obligatorio");
        }

        if (Input.FechaPublicacion.HasValue && Input.FechaPublicacion.Value > DateTime.Now)
        {
            ModelState.AddModelError("Input.FechaPublicacion", "La fecha de publicación no puede ser futura");
        }

        if (!string.IsNullOrWhiteSpace(Input.Isbn))
        {
            var libroExistente = await _svc.ObtenerTodos();
            if (libroExistente.Any(l => l.Isbn == Input.Isbn && l.Id != Id))
            {
                ModelState.AddModelError("Input.Isbn", "Ya existe un libro con este ISBN");
            }
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        Input.Id = Id;
        Input.UltimaActualizacion = DateTime.Now;
        await _svc.Actualizar(Input);
        return RedirectToPage("Index");
    }
}
