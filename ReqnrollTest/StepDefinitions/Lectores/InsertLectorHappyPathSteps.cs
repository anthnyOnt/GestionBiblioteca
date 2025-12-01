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
using GestionBiblioteca.Repository;
using Reqnroll;
using Reqnroll.Assist;

namespace ReqnrollTest.StepDefinitions.Lectores;

[Binding]
public class InsertLectorHappyPathSteps
{
    private readonly ServiceProvider _serviceProvider;
    private readonly MyDbContext _context;
    private readonly IUsuarioService _usuarioService;
    
    private Usuario _testLector;
    private Usuario _createdLector;
    private bool _userAuthorized;
    
    public InsertLectorHappyPathSteps()
    {
        // Setup in-memory database for integration testing
        var services = new ServiceCollection();
        
        services.AddDbContext<MyDbContext>(options =>
        {
            options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());
            // Ignore warnings for in-memory database - it doesn't support transactions
            options.ConfigureWarnings(w => w.Default(WarningBehavior.Ignore));
        });
        
        // Register repository factory
        services.AddScoped<IRepositoryFactory, RepositoryFactory>();
        
        // Register services
        services.AddScoped<IUsuarioService, UsuarioService>();
        
        // Add HttpContextAccessor for services that need it
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        
        _serviceProvider = services.BuildServiceProvider();
        _context = _serviceProvider.GetRequiredService<MyDbContext>();
        _usuarioService = _serviceProvider.GetRequiredService<IUsuarioService>();
        
        // Ensure database is created
        _context.Database.EnsureCreated();
    }

    [Given(@"que soy un usuario autorizado del sistema")]
    public void GivenQueSoyUnUsuarioAutorizadoDelSistema()
    {
        // Simulate that the user is authorized to perform operations
        _userAuthorized = true;
        _userAuthorized.Should().BeTrue("User should be authorized to use the system");
    }

    [Given(@"que no existe un lector con CI ""(.*)""")]
    public async Task GivenQueNoExisteUnLectorConCI(string ci)
    {
        // Verify that no user with this CI already exists
        var existingUser = await _usuarioService.ObtenerPorCi(ci);
        existingUser.Should().BeNull($"No user should exist with CI {ci} before creating a new one");
    }

    [When(@"registro un nuevo lector con los siguientes datos:")]
    public async Task WhenRegistroUnNuevoLectorConLosSiguientesDatos(Table table)
    {
        // Arrange - Extract data from the table
        var lectorData = table.CreateInstance<LectorData>();
        
        // Create the lector object
        _testLector = new Usuario
        {
            Ci = lectorData.CI,
            PrimerNombre = lectorData.PrimerNombre,
            SegundoNombre = lectorData.SegundoNombre,
            PrimerApellido = lectorData.PrimerApellido,
            SegundoApellido = lectorData.SegundoApellido,
            Telefono = lectorData.Telefono,
            Correo = lectorData.Correo
        };

        // Act - Create the lector using the service
        _createdLector = await _usuarioService.AgregarLector(_testLector);
        
        // Basic verification that the operation completed
        _createdLector.Should().NotBeNull("Lector creation should not return null");
    }

    [Then(@"el lector se guarda correctamente")]
    public async Task ThenElLectorSeGuardaCorrectamente()
    {
        // Verify the lector was saved by retrieving it from the database
        var savedLector = await _usuarioService.ObtenerPorCi(_testLector.Ci);
        savedLector.Should().NotBeNull("Lector should be found in the database");
        savedLector.Ci.Should().Be(_testLector.Ci);
        savedLector.PrimerNombre.Should().Be(_testLector.PrimerNombre);
        savedLector.SegundoNombre.Should().Be(_testLector.SegundoNombre);
        savedLector.PrimerApellido.Should().Be(_testLector.PrimerApellido);
        savedLector.SegundoApellido.Should().Be(_testLector.SegundoApellido);
        savedLector.Telefono.Should().Be(_testLector.Telefono);
        savedLector.Correo.Should().Be(_testLector.Correo);
    }

    [Then(@"el sistema asigna un ID único al lector")]
    public void ThenElSistemaAsignaUnIDUnicoAlLector()
    {
        // Verify that the created lector has a valid ID
        _createdLector.Id.Should().BeGreaterThan(0, "Lector should have a valid ID assigned by the system");
        _testLector.Id.Should().BeGreaterThan(0, "Test lector should have been updated with the assigned ID");
    }

    [Then(@"el lector queda con estado ""(.*)""")]
    public void ThenElLectorQuedaConEstado(string estado)
    {
        // Verify the lector's status
        if (estado == "Activo")
        {
            _createdLector.Activo.Should().Be(1, "Lector should be active (Activo = 1)");
        }
        else
        {
            _createdLector.Activo.Should().Be(0, "Lector should be inactive (Activo = 0)");
        }
    }

    [Then(@"el lector tiene rol ""(.*)""")]
    public void ThenElLectorTieneRol(string rolEsperado)
    {
        // Verify the lector's role
        _createdLector.Rol.Should().Be(rolEsperado, $"Lector should have role '{rolEsperado}'");
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

// Helper class for table data binding
public class LectorData
{
    public string CI { get; set; }
    public string PrimerNombre { get; set; }
    public string SegundoNombre { get; set; }
    public string PrimerApellido { get; set; }
    public string SegundoApellido { get; set; }
    public string Telefono { get; set; }
    public string Correo { get; set; }
}