using System;
using System.Threading.Tasks;
using GestionBiblioteca.Services.Usuario;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GestionBiblioteca.Pages.Usuario;

public class Edit : PageModel
{
    private readonly IUsuarioService _svc;
    public Edit(IUsuarioService svc) { _svc = svc; }

    [FromRoute] public int Id { get; set; }
    [BindProperty] public Entities.Usuario Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var u = await _svc.ObtenerPorId(id);
        if (u == null) return RedirectToPage("Index");
        Input = u;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        Input.Id = Id;
        Input.UltimaActualizacion = DateTime.Now;
        await _svc.Actualizar(Input);
        return RedirectToPage("Index");
    }
}
