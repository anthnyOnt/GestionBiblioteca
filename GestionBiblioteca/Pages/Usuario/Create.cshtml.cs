using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using GestionBiblioteca.Context;
using GestionBiblioteca.Entities;
using GestionBiblioteca.Services.Usuario;

namespace GestionBiblioteca
{
    public class CreateModel : PageModel
    {
        private readonly IUsuarioService _service;

        public CreateModel(IUsuarioService service)
        {
            _service = service;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Usuario Usuario { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _service.Crear(Usuario);
            return RedirectToPage("./Index");
        }
    }
}
