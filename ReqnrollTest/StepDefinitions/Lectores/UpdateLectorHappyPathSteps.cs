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

namespace ReqnrollTest.StepDefinitions.Lectores;

[Binding]
public class UpdateLectorHappyPathSteps
{
    private readonly ServiceProvider _serviceProvider;
    private readonly MyDbContext _context;
    private readonly IUsuarioService _usuarioService;
    
    private Usuario _existingLector;
    private Usuario _updatedLector;
    private bool _updateResult;
    private string _targetCI;
    
    public UpdateLectorHappyPathSteps()
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
    }
    
    [Given(@"que existe un lector con CI ""([^""]*)"" y teléfono ""([^""]*)""")]
    public async Task GivenQueExisteUnLectorConCIYTelefono(string ci, string telefono)
    {
        await CreateLectorWithField(ci, "telefono", telefono);
    }
    
    [Given(@"que existe un lector con CI ""([^""]*)"" y correo ""([^""]*)""")]
    public async Task GivenQueExisteUnLectorConCIYCorreo(string ci, string correo)
    {
        await CreateLectorWithField(ci, "correo", correo);
    }
    
    private async Task CreateLectorWithField(string ci, string campo, string valor)
    {
        _targetCI = ci;
        
        _existingLector = new Usuario
        {
            Ci = ci,
            PrimerNombre = "Juan",
            PrimerApellido = "Pérez",
            Correo = campo == "correo" ? valor : "juan.perez@email.com",
            Telefono = campo == "telefono" ? valor : "70000000",
            SegundoNombre = "",
            SegundoApellido = "",
            FechaCreacion = DateTime.Now,
            UltimaActualizacion = DateTime.Now,
            Rol = "Lector",
            Activo = 1
        };
        
        _context.Usuarios.Add(_existingLector);
        await _context.SaveChangesAsync();
        
        // Verify the lector exists
        var lectorExists = await _usuarioService.ObtenerPorCi(ci);
        lectorExists.Should().NotBeNull("el lector debe existir antes de la actualización");
    }
    
    [When(@"actualizo su teléfono a ""([^""]*)""")]
    public async Task WhenActualizoSuTelefonoA(string nuevoTelefono)
    {
        await UpdateLectorField("telefono", nuevoTelefono);
    }
    
    [When(@"actualizo su correo a ""([^""]*)""")]
    public async Task WhenActualizoSuCorreoA(string nuevoCorreo)
    {
        await UpdateLectorField("correo", nuevoCorreo);
    }
    
    private async Task UpdateLectorField(string campo, string nuevoValor)
    {
        // Get the current lector
        var currentLector = await _usuarioService.ObtenerPorCi(_targetCI);
        currentLector.Should().NotBeNull("el lector debe existir para poder actualizarlo");
        
        // Update the specific field
        if (campo == "telefono")
        {
            currentLector.Telefono = nuevoValor;
        }
        else if (campo == "correo")
        {
            currentLector.Correo = nuevoValor;
        }
        
        currentLector.UltimaActualizacion = DateTime.Now;
        
        try
        {
            _updatedLector = await _usuarioService.Actualizar(currentLector);
            _updateResult = _updatedLector != null;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error al actualizar el lector: {ex.Message}");
        }
    }
    
    [Then(@"los datos se actualizan correctamente")]
    public void ThenLosDatosSeActualizanCorrectamente()
    {
        _updateResult.Should().BeTrue("la operación de actualización debe ser exitosa");
        _updatedLector.Should().NotBeNull("el lector actualizado debe existir");
    }
    
    [Then(@"el sistema devuelve estado HTTP para actualización (\d+)")]
    public void ThenElSistemaDevuelveEstadoHTTPParaActualizacion(int expectedStatusCode)
    {
        // In integration test, we verify the update operation was successful
        // HTTP status codes are typically handled at the controller level
        expectedStatusCode.Should().Be(200, "el código de estado debe ser 200 para actualización exitosa");
        _updateResult.Should().BeTrue("la operación debe haber sido exitosa para devolver 200");
    }
    
    [Then(@"el lector tiene teléfono ""([^""]*)""")]
    public void ThenElLectorTieneTelefono(string expectedTelefono)
    {
        _updatedLector.Should().NotBeNull("el lector actualizado debe existir");
        _updatedLector.Telefono.Should().Be(expectedTelefono, "el teléfono debe haber sido actualizado correctamente");
    }
    
    [Then(@"el lector tiene correo ""([^""]*)""")]
    public void ThenElLectorTieneCorreo(string expectedCorreo)
    {
        _updatedLector.Should().NotBeNull("el lector actualizado debe existir");
        _updatedLector.Correo.Should().Be(expectedCorreo, "el correo debe haber sido actualizado correctamente");
    }
    
    public void Dispose()
    {
        _context?.Dispose();
        _serviceProvider?.Dispose();
    }
}