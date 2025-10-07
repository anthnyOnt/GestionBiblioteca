using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GestionBiblioteca.Entities;
using GestionBiblioteca.Services.Usuario;

namespace GestionBiblioteca
{
    public class EditModel : PageModel
    {
        private readonly IUsuarioService _service;

        public EditModel(IUsuarioService service)
        {
            _service = service;
        }

        [BindProperty]
        public Usuario Usuario { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _service.ObtenerPorId((int)id);
            if (usuario == null)
            {
                return NotFound();
            }
            Usuario = usuario;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var updatedUsuario = await _service.Actualizar(Usuario);
            }
            catch (Exception ex)
            {
                if (!UsuarioExists(Usuario.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool UsuarioExists(int id)
        {
            return _service.ObtenerPorId(id) != null;
        }
    }
}
