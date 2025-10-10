using System.Threading.Tasks;
using GestionBiblioteca.Services.Usuario;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GestionBiblioteca.Pages.Usuario;

public class Delete : PageModel
{
    private readonly IUsuarioService _svc;
    public Delete(IUsuarioService svc) { _svc = svc; }

    [FromRoute] public int Id { get; set; }
    public string Nombre { get; private set; } = "";

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var u = await _svc.ObtenerPorId(id);
        if (u == null) return RedirectToPage("Index");
        Nombre = $"{u.PrimerNombre} {u.PrimerApellido}";
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _svc.Eliminar(Id);
        return RedirectToPage("Index");
    }
}
