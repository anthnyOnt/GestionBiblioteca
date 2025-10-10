using System.Collections.Generic;
using System.Threading.Tasks;
using GestionBiblioteca.Services.Usuario;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GestionBiblioteca.Pages.Usuario;

public class Index : PageModel
{
    private readonly IUsuarioService _svc;
    public Index(IUsuarioService svc) { _svc = svc; }

    public IList<Entities.Usuario> Usuarios { get; private set; } = new List<Entities.Usuario>();

    public async Task OnGetAsync()
    {
        Usuarios = await _svc.ObtenerTodos();
    }
}
