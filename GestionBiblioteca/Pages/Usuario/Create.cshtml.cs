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
        // Manual validation for required fields
        if (string.IsNullOrWhiteSpace(Input.PrimerNombre))
        {
            ModelState.AddModelError("Input.PrimerNombre", "El primer nombre es obligatorio");
        }
        else if (Input.PrimerNombre.Length < 3 || Input.PrimerNombre.Length > 25)
        {
            ModelState.AddModelError("Input.PrimerNombre", "El primer nombre debe contener entre 3 a 25 caracteres");
        }
        
        if (string.IsNullOrWhiteSpace(Input.PrimerApellido))
        {
            ModelState.AddModelError("Input.PrimerApellido", "El primer apellido es obligatorio");
        }
        else if (Input.PrimerApellido.Length < 3 || Input.PrimerApellido.Length > 25)
        {
            ModelState.AddModelError("Input.PrimerApellido", "El primer apellido debe contener entre 3 a 25 caracteres");
        }

        // Validate CI
        if (string.IsNullOrWhiteSpace(Input.Ci))
        {
            ModelState.AddModelError("Input.Ci", "El número de cédula es obligatorio");
        }
        else if (Input.Ci.Length < 6 || Input.Ci.Length > 10)
        {
            ModelState.AddModelError("Input.Ci", "El número de cédula debe contener entre 6 a 10 dígitos");
        }
        
        // Validate Telefono
        if (string.IsNullOrWhiteSpace(Input.Telefono))
        {
            ModelState.AddModelError("Input.Telefono", "El número de teléfono es obligatorio");
        }
        else if (Input.Telefono.Length < 8 || Input.Telefono.Length > 10)
        {
            ModelState.AddModelError("Input.Telefono", "El teléfono debe contener entre 8 a 10 caracteres");
        }
        
        // Validate Correo
        if (string.IsNullOrWhiteSpace(Input.Correo))
        {
            ModelState.AddModelError("Input.Correo", "El correo electrónico es obligatorio");
        }
        else if (Input.Correo.Length < 5 || Input.Correo.Length > 45)
        {
            ModelState.AddModelError("Input.Correo", "El correo electrónico debe contener entre 5 a 45 caracteres");
        }
        
        // Optional field validations
        if (!string.IsNullOrWhiteSpace(Input.SegundoNombre) && 
            (Input.SegundoNombre.Length < 3 || Input.SegundoNombre.Length > 25))
        {
            ModelState.AddModelError("Input.SegundoNombre", "El segundo nombre debe contener entre 3 a 25 caracteres");
        }
        
        if (!string.IsNullOrWhiteSpace(Input.SegundoApellido) && 
            (Input.SegundoApellido.Length < 3 || Input.SegundoApellido.Length > 25))
        {
            ModelState.AddModelError("Input.SegundoApellido", "El segundo apellido debe contener entre 3 a 25 caracteres");
        }

        if (!ModelState.IsValid) return Page();
        
        Input.Rol = "lector";
        Input.Activo = 1;
        Input.FechaCreacion = DateTime.Now;
        Input.UltimaActualizacion = DateTime.Now;

        await _svc.Crear(Input);
        return RedirectToPage("Index");
    }
}
