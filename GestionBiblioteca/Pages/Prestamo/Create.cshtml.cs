using System.Runtime.InteropServices.JavaScript;
using GestionBiblioteca.Entities;
using GestionBiblioteca.Pages.Prestamo;
using GestionBiblioteca.Services.Ejemplar;
using GestionBiblioteca.Services.Libro;
using GestionBiblioteca.Services.Prestamo;
using GestionBiblioteca.Services.Usuario;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GestionBiblioteca.Pages.Prestamo;

public class Create : PageModel
{
    private readonly IUsuarioService _usuarioService;
    private readonly ILibroService _libroService;
    private readonly IEjemplarService _ejemplarService;
    private readonly IPrestamoService _prestamoService;

    public Create(IUsuarioService usuarioService, ILibroService libroService, IEjemplarService ejemplarService, IPrestamoService prestamoService)
    {
        _usuarioService = usuarioService;
        _libroService = libroService;
        _ejemplarService = ejemplarService;
        _prestamoService = prestamoService;
    }

    [BindProperty(SupportsGet = true)]
    public string Ci { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public string Titulo { get; set; } = string.Empty;

    public Entities.Usuario? UsuarioEncontrado { get; set; }
    public List<Libro>? LibrosEncontrados { get; set; }

    // Temp list to store selected ejemplares
    public List<int> EjemplaresSeleccionados
    {
        get => HttpContext.Session.GetObjectFromJson<List<int>>("EjemplaresSeleccionados") ?? new List<int>();
        set => HttpContext.Session.SetObjectAsJson("EjemplaresSeleccionados", value);
    }

    public List<Ejemplar> EjemplaresSeleccionadosConDatos { get; set; } = new();
    
    public async Task OnGetAsync()
    {
        if (!string.IsNullOrWhiteSpace(Ci))
        {
            UsuarioEncontrado = await _usuarioService.ObtenerPorCi(Ci);
        }

        if (!string.IsNullOrWhiteSpace(Titulo))
        {
            LibrosEncontrados = await _libroService.ObtenerEjemplaresPorTitulo(Titulo);
        }
        
        if (EjemplaresSeleccionados.Any())
        {
            EjemplaresSeleccionadosConDatos = await _ejemplarService.ObtenerSeleccionados(EjemplaresSeleccionados);
        }
    }

    public IActionResult OnPostAgregarEjemplar(int ejemplarId)
    {
        var lista = EjemplaresSeleccionados;
        if (!lista.Contains(ejemplarId))
            lista.Add(ejemplarId);

        EjemplaresSeleccionados = lista;
        return RedirectToPage(new { Ci, Titulo });
    }

    public IActionResult OnPostQuitarEjemplar(int ejemplarId)
    {
        var lista = EjemplaresSeleccionados;
        lista.Remove(ejemplarId);

        EjemplaresSeleccionados = lista;
        return RedirectToPage(new { Ci, Titulo });
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // 1️⃣ Validate that a user was found
        var usuario = await _usuarioService.ObtenerPorCi(Ci);
        if (usuario == null)
        {
            ModelState.AddModelError("", "El usuario no existe.");
            return Page();
        }

        // 2️⃣ Validate that at least one ejemplar was selected
        var ejemplarIds = EjemplaresSeleccionados;
        if (ejemplarIds == null || !ejemplarIds.Any())
        {
            ModelState.AddModelError("", "Debe seleccionar al menos un ejemplar.");
            return Page();
        }

        // 3️⃣ Create Prestamo entity
        var nuevoPrestamo = new Entities.Prestamo
        {
            IdUsuario = usuario.Id,
            FechaPrestamo = DateTime.Now,
            Activo = 1
        };

        // 4️⃣ Call service to insert everything in a transaction
        await _prestamoService.Crear(nuevoPrestamo, ejemplarIds);

        // 5️⃣ Clear selected ejemplares (optional)
        EjemplaresSeleccionados = new List<int>();

        // 6️⃣ Redirect or show confirmation
        TempData["SuccessMessage"] = "Préstamo registrado correctamente.";
        return RedirectToPage("./Index");
    }
}