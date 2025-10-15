using System;
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
        if (!ModelState.IsValid) return Page();
        Input.Activo = 1;
        Input.FechaCreacion = DateTime.Now;
        Input.UltimaActualizacion = DateTime.Now;

        await _svc.Crear(Input);
        return RedirectToPage("Index");
    }
}
