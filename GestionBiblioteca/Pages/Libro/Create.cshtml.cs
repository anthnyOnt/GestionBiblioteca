using System;
using System.Linq;
using System.Threading.Tasks;
using GestionBiblioteca.Services.Libro;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GestionBiblioteca.Pages.Libro;

public class Create : PageModel
{
    private readonly ILibroService _svc;
    public Create(ILibroService svc) { _svc = svc; }

    [BindProperty] public Entities.Libro Input { get; set; } = new();

    public void OnGet() { }

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
            if (libroExistente.Any(l => l.Isbn == Input.Isbn))
            {
                ModelState.AddModelError("Input.Isbn", "Ya existe un libro con este ISBN");
            }
        }

        if (!ModelState.IsValid) return Page();
        
        Input.Activo = 1;
        Input.FechaCreacion = DateTime.Now;
        Input.UltimaActualizacion = DateTime.Now;

        await _svc.Crear(Input);
        return RedirectToPage("Index");
    }
}
