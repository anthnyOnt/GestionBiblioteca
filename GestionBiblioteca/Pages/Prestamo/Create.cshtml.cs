using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Ent = GestionBiblioteca.Entities;
using IUService = GestionBiblioteca.Services.Usuario.IUsuarioService;
using GestionBiblioteca.Services.Libro;
using GestionBiblioteca.Services.Ejemplar;
using GestionBiblioteca.Services.Prestamo;
using GestionBiblioteca.Services.PrestamoDraftCache;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GestionBiblioteca.Pages.Prestamo;

public class Create : PageModel
{
    public sealed class LineaInput
    {
        public int EjemplarId { get; set; }
        public DateTime? FechaLimite { get; set; }
    }

    private readonly IUService _usuarioService;
    private readonly ILibroService _libroService;
    private readonly IEjemplarService _ejemplarService;
    private readonly IPrestamoService _prestamoService;
    private readonly IPrestamoDraftCache _draft;

    public Create(IUService usuarioService, ILibroService libroService,
                  IEjemplarService ejemplarService, IPrestamoService prestamoService,
                  IPrestamoDraftCache draft)
    {
        _usuarioService = usuarioService;
        _libroService = libroService;
        _ejemplarService = ejemplarService;
        _prestamoService = prestamoService;
        _draft = draft;
    }

    [BindProperty(SupportsGet = true)]
    [Required(ErrorMessage = "Introduzca un número de CI")]
    public string Ci { get; set; } = string.Empty;
    
    [BindProperty(SupportsGet = true)] 
    [Required(ErrorMessage = "Introduzca un título")]
    public string Titulo { get; set; } = string.Empty;

    [BindProperty] public List<LineaInput> Lineas { get; set; } = new();

    public Ent.Usuario? UsuarioEncontrado { get; private set; }
    public List<Ent.Libro>? LibrosEncontrados { get; private set; }
    public List<Ent.Ejemplar> EjemplaresSeleccionadosConDatos { get; private set; } = new();

    public async Task OnGetAsync()
    {
        if (!string.IsNullOrWhiteSpace(Ci))
            UsuarioEncontrado = await _usuarioService.ObtenerPorCi(Ci);

        if (!string.IsNullOrWhiteSpace(Titulo))
            LibrosEncontrados = await _libroService.ObtenerEjemplaresPorTitulo(Titulo);

        var ids = await _draft.GetAllAsync(UsuarioEncontrado?.Id ?? 0);

        if (ids.Count > 0)
        {
            EjemplaresSeleccionadosConDatos = await _ejemplarService.ObtenerSeleccionados(ids.ToList());

            if (Lineas.Count == 0)
            {
                var porDefecto = DateTime.Today.AddDays(3);
                Lineas = EjemplaresSeleccionadosConDatos
                    .Select(e => new LineaInput { EjemplarId = e.Id, FechaLimite = porDefecto })
                    .ToList();
            }
            else
            {
                var map = Lineas.ToDictionary(x => x.EjemplarId, x => x.FechaLimite);
                Lineas = EjemplaresSeleccionadosConDatos
                    .Select(e => new LineaInput
                    {
                        EjemplarId = e.Id,
                        FechaLimite = map.TryGetValue(e.Id, out var f) ? f : DateTime.Today.AddDays(3)
                    })
                    .ToList();
            }
        }
    }

    public async Task<IActionResult> OnPostAgregarEjemplar(int ejemplarId, string ci, string titulo)
    {
        Ci = ci; Titulo = titulo;
        var u = await _usuarioService.ObtenerPorCi(Ci);
        if (u is null) return RedirectToPage(new { Ci, Titulo });

        await _draft.AddAsync(u.Id, ejemplarId);
        return RedirectToPage(new { Ci, Titulo });
    }

    public async Task<IActionResult> OnPostQuitarEjemplar(int ejemplarId, string ci, string titulo)
    {
        Ci = ci; Titulo = titulo;
        var u = await _usuarioService.ObtenerPorCi(Ci);
        if (u is null) return RedirectToPage(new { Ci, Titulo });

        await _draft.RemoveAsync(u.Id, ejemplarId);
        return RedirectToPage(new { Ci, Titulo });
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(Ci))
        {
            ModelState.AddModelError(string.Empty, "Debe ingresar CI.");
            await OnGetAsync();
            return Page();
        }
        var u = await _usuarioService.ObtenerPorCi(Ci);
        if (u is null)
        {
            ModelState.AddModelError(string.Empty, "El usuario no existe.");
            await OnGetAsync();
            return Page();
        }

        var ids = await _draft.GetAllAsync(u.Id);
        if (ids.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Seleccione al menos un ejemplar.");
            await OnGetAsync();
            return Page();
        }

        // Validaci�n por l�nea
        if (Lineas == null || Lineas.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Debe definir fechas por ejemplar.");
            await OnGetAsync();
            return Page();
        }

        foreach (var l in Lineas)
        {
            if (l.FechaLimite is null || l.FechaLimite.Value.Date < DateTime.Today)
                ModelState.AddModelError(string.Empty, $"Fecha l�mite inv�lida para ejemplar #{l.EjemplarId}.");
        }
        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }

        // Construir l�neas reales
        var lineas = Lineas.Select(l => new Ent.PrestamoEjemplar
        {
            IdEjemplar = l.EjemplarId,
            FechaLimite = l.FechaLimite,
            Activo = 1
        }).ToList();

        await _prestamoService.Crear(u.Id, lineas);
        await _draft.ClearAsync(u.Id);

        TempData["SuccessMessage"] = "Pr�stamo creado correctamente.";
        return RedirectToPage("./Index");
    }
}
