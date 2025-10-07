using Microsoft.AspNetCore.Mvc.RazorPages;
using GestionBiblioteca.Services.Usuario;

namespace GestionBiblioteca.Pages.Usuario
{
    public class IndexModel : PageModel
    {
        private readonly IUsuarioService _service;

        public IndexModel(IUsuarioService service)
        {
            _service = service;
        }

        public IList<Entities.Usuario> Usuario { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Usuario = await _service.ObtenerTodos();
        }
    }
}
