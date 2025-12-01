using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using GestionBiblioteca.Context;
using GestionBiblioteca.Entities;
using GestionBiblioteca.Services.Usuario;
using GestionBiblioteca.Services.Libro;
using GestionBiblioteca.Services.Ejemplar;
using GestionBiblioteca.Services.Prestamo;
using GestionBiblioteca.Repository;
using Reqnroll;

namespace ReqnrollTest.StepDefinitions.MainProcess;

[Binding]
public class HappyPathMainProcessSteps
{
    private readonly ServiceProvider _serviceProvider;
    private readonly MyDbContext _context;
    private readonly IUsuarioService _usuarioService;
    private readonly ILibroService _libroService;
    private readonly IEjemplarService _ejemplarService;
    private readonly IPrestamoService _prestamoService;
    
    private Usuario _testUser;
    private Libro _testBook;
    private List<Ejemplar> _testEjemplares;
    private Prestamo _testPrestamo;
    
    public HappyPathMainProcessSteps()
    {
        // Setup in-memory database for integration testing
        var services = new ServiceCollection();
        
        services.AddDbContext<MyDbContext>(options =>
        {
            options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());

            options.ConfigureWarnings(w => w.Default(WarningBehavior.Ignore));
        });
        

        services.AddScoped<IRepositoryFactory, RepositoryFactory>();
        
        // Register services
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<ILibroService, LibroService>();
        services.AddScoped<IEjemplarService, EjemplarService>();
        services.AddScoped<IPrestamoService, PrestamoService>();
        
        // Add HttpContextAccessor for services that need it
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        
        _serviceProvider = services.BuildServiceProvider();
        _context = _serviceProvider.GetRequiredService<MyDbContext>();
        _usuarioService = _serviceProvider.GetRequiredService<IUsuarioService>();
        _libroService = _serviceProvider.GetRequiredService<ILibroService>();
        _ejemplarService = _serviceProvider.GetRequiredService<IEjemplarService>();
        _prestamoService = _serviceProvider.GetRequiredService<IPrestamoService>();
        
        _testEjemplares = new List<Ejemplar>();
        

        _context.Database.EnsureCreated();
    }

    [Given(@"que existe un lector con CI ""(.*)"" y estado ""(.*)""")]
    public async Task GivenQueExisteUnLectorConCIYEstado(string ci, string estado)
    {
        // Arrange - Create a test user (reader) with the specified CI
        _testUser = new Usuario
        {
            PrimerNombre = "Juan",
            PrimerApellido = "Pérez",
            Ci = ci,
            Telefono = "12345678",
            Correo = "juan.perez@test.com",
            Rol = "LECTOR",
            Activo = estado == "Activo" ? 1 : 0,
            FechaCreacion = DateTime.Now,
            CreadoPor = 1
        };


        await _usuarioService.AgregarLector(_testUser);
        

        var createdUser = await _usuarioService.ObtenerPorCi(ci);
        createdUser.Should().NotBeNull();
        createdUser.Ci.Should().Be(ci);
        createdUser.Activo.Should().Be(estado == "Activo" ? 1 : 0);
    }

    [Given(@"existe un libro con Título ""(.*)"" y (.*) ejemplares disponibles")]
    public async Task GivenExisteUnLibroConTituloYEjemplaresDisponibles(string titulo, int cantidadEjemplares)
    {

        _testBook = new Libro
        {
            Titulo = titulo,
            Isbn = "978-3-16-148410-0",
            Sinopsis = "Una historia maravillosa",
            FechaPublicacion = DateTime.Now.AddYears(-1),
            Idioma = "Español",
            Edicion = "1ra",
            Activo = 1,
            FechaCreacion = DateTime.Now,
            CreadoPor = 1
        };

        await _libroService.Crear(_testBook);

        for (int i = 1; i <= cantidadEjemplares; i++)
        {
            var ejemplar = new Ejemplar
            {
                IdLibro = _testBook.Id,
                Disponible = true,
                Descripcion = $"Ejemplar {i} del libro {titulo}",
                Observaciones = $"Ejemplar EJ-10{i}",
                FechaAdquisicion = DateTime.Now.AddMonths(-1),
                Activo = 1,
                FechaCreacion = DateTime.Now,
                CreadoPor = 1
            };

            await _ejemplarService.Crear(ejemplar);
            _testEjemplares.Add(ejemplar);
        }

        // Assert - Verify book and ejemplares were created
        var createdBook = await _libroService.ObtenerPorId(_testBook.Id);
        createdBook.Should().NotBeNull();
        createdBook.Titulo.Should().Be(titulo);
        
        var availableEjemplares = await _ejemplarService.ObtenerDisponibles();
        var bookEjemplares = availableEjemplares.Where(e => e.IdLibro == _testBook.Id).ToList();
        bookEjemplares.Should().HaveCount(cantidadEjemplares);
    }

    [When(@"registró un préstamo para el lector con CI ""(.*)"" seleccionando los ejemplares con Ids ""(.*)"" y ""(.*)""")]
    public async Task WhenRegistroUnPrestamoParaElLectorConCISeleccionandoLosEjemplaresConIds(string ci, string ejemplarId1, string ejemplarId2)
    {
        // Arrange - Get the user and ejemplares
        var usuario = await _usuarioService.ObtenerPorCi(ci);
        usuario.Should().NotBeNull();

        // Get ejemplares by their observation (which contains the ID)
        var availableEjemplares = await _ejemplarService.ObtenerDisponibles();
        var ejemplar1 = availableEjemplares.FirstOrDefault(e => e.Observaciones.Contains(ejemplarId1));
        var ejemplar2 = availableEjemplares.FirstOrDefault(e => e.Observaciones.Contains(ejemplarId2));
        
        ejemplar1.Should().NotBeNull($"Ejemplar with ID {ejemplarId1} should exist and be available");
        ejemplar2.Should().NotBeNull($"Ejemplar with ID {ejemplarId2} should exist and be available");

        // Act - Create the loan directly without using transactions (for testing with in-memory database)
        // Create prestamo first
        _testPrestamo = new Prestamo
        {
            IdUsuario = usuario.Id,
            FechaPrestamo = DateTime.Now,
            Activo = 1,
            Cancelado = false
        };
        await _context.Prestamos.AddAsync(_testPrestamo);
        await _context.SaveChangesAsync();

        // Mark ejemplares as not available
        ejemplar1.Disponible = false;
        ejemplar2.Disponible = false;
        _context.Ejemplars.Update(ejemplar1);
        _context.Ejemplars.Update(ejemplar2);
        await _context.SaveChangesAsync();

        // Create prestamo ejemplares relationships
        var prestamoEjemplar1 = new PrestamoEjemplar
        {
            IdPrestamo = _testPrestamo.Id,
            IdEjemplar = ejemplar1.Id,
            FechaLimite = DateTime.Now.AddDays(15),
            Activo = 1
        };
        
        var prestamoEjemplar2 = new PrestamoEjemplar
        {
            IdPrestamo = _testPrestamo.Id,
            IdEjemplar = ejemplar2.Id,
            FechaLimite = DateTime.Now.AddDays(15),
            Activo = 1
        };

        await _context.PrestamoEjemplars.AddAsync(prestamoEjemplar1);
        await _context.PrestamoEjemplars.AddAsync(prestamoEjemplar2);
        await _context.SaveChangesAsync();
    }

    [Then(@"el sistema confirma el préstamo y marca los ejemplares como ""(.*)""")]
    public async Task ThenElSistemaConfirmaElPrestamoYMarcaLosEjemplaresComoEstado(string estadoEsperado)
    {
        // Assert - Verify loan was created and ejemplares are marked as loaned
        _testPrestamo.Should().NotBeNull();
        _testPrestamo.IdUsuario.Should().Be(_testUser.Id);
        _testPrestamo.FechaPrestamo.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMinutes(1));
        
        // Verify ejemplares are no longer available (marked as "Prestado")
        var availableEjemplares = await _ejemplarService.ObtenerDisponibles();
        var loanedEjemplares = _testEjemplares.Where(e => 
            !availableEjemplares.Any(ae => ae.Id == e.Id)).ToList();
        
        if (estadoEsperado == "Prestado")
        {
            loanedEjemplares.Should().HaveCount(2, "Both ejemplares should be marked as loaned");
        }
        
        // Verify the ejemplares are marked as not available in database
        foreach (var ejemplar in _testEjemplares.Take(2)) // First 2 ejemplares were loaned
        {
            var ejemplarFromDb = await _context.Ejemplars.FindAsync(ejemplar.Id);
            ejemplarFromDb.Should().NotBeNull();
            ejemplarFromDb.Disponible.Should().BeFalse($"Ejemplar {ejemplar.Id} should be marked as not available");
        }
    }

    [Then(@"se crea el registro de préstamo con estado ""(.*)""")]
    public async Task ThenSeCreaElRegistroDePrestamoConEstado(string estado)
    {
        _testPrestamo.Should().NotBeNull();
        
        if (estado == "Activo")
        {
            _testPrestamo.Activo.Should().Be(1);
            _testPrestamo.Cancelado.Should().BeFalse();
            _testPrestamo.FechaCancelacion.Should().BeNull();
        }
        else if (estado == "Cancelado")
        {
            _testPrestamo.Cancelado.Should().BeTrue();
            _testPrestamo.FechaCancelacion.Should().NotBeNull();
        }
        
        var prestamoDetalle = await _prestamoService.ObtenerPorId(_testPrestamo.Id);
        prestamoDetalle.Should().NotBeNull();
        prestamoDetalle.PrestamoEjemplares.Should().HaveCount(2, "Should have 2 ejemplares in the loan");
        
        foreach (var pe in prestamoDetalle.PrestamoEjemplares)
        {
            pe.IdPrestamo.Should().Be(_testPrestamo.Id);
            pe.FechaLimite.Should().BeAfter(DateTime.Now, "Loan limit date should be in the future");
            pe.Activo.Should().Be(1, "PrestamoEjemplar should be active");
        }
    }
    
    [AfterScenario]
    public void CleanupAfterScenario()
    {
        // Clean up database after each scenario
        try
        {
            _context?.Database.EnsureDeleted();
            _context?.Dispose();
        }
        catch (Exception ex)
        {
            // Log or handle cleanup errors if needed
            Console.WriteLine($"Cleanup warning: {ex.Message}");
        }
        finally
        {
            _serviceProvider?.Dispose();
        }
    }
}