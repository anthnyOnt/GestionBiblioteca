using GestionBiblioteca.Services.Usuario;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GestionBiblioteca.Pages.Lector
{
    public class Create : PageModel
    {
        private readonly IUsuarioService _service;

        public Create(IUsuarioService service)
        {
            _service = service;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Entities.Usuario Lector { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _service.AgregarLector(Lector);
            return RedirectToPage("./Index");
        }
    }
}
