using System;
using System.Threading.Tasks;
using GestionBiblioteca.Services.Usuario;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GestionBiblioteca.Pages.Usuario;

public class Create : PageModel
{
    private readonly IUsuarioService _svc;
    public Create(IUsuarioService svc) { _svc = svc; }

    [BindProperty] public Entities.Usuario Input { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        Input.Rol = "lector";
        Input.Activo = 1;
        Input.FechaCreacion = DateTime.Now;
        Input.UltimaActualizacion = DateTime.Now;

        await _svc.Crear(Input);
        return RedirectToPage("Index");
    }
}
