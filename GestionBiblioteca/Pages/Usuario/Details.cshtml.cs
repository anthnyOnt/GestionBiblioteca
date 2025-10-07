using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GestionBiblioteca.Context;
using GestionBiblioteca.Entities;
using GestionBiblioteca.Services.Usuario;

namespace GestionBiblioteca
{
    public class DetailsModel : PageModel
    {
        private readonly IUsuarioService _service;

        public DetailsModel(IUsuarioService service)
        {
            _service = service;
        }

        public Usuario Usuario { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _service.ObtenerPorId(id.Value);

            if (usuario is not null)
            {
                Usuario = usuario;

                return Page();
            }

            return NotFound();
        }
    }
}
