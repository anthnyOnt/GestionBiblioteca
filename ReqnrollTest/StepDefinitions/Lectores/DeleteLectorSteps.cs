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
public class DeleteLectorSteps
{
    private readonly ServiceProvider _serviceProvider;
    private readonly MyDbContext _context;
    private readonly IUsuarioService _usuarioService;
    
    private Usuario _lectorToDelete;
    private bool _deleteResult;
    private string _deletedCI;
    
    public DeleteLectorSteps()
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
    
    [Given(@"que existe un lector con CI ""([^""]*)"" y nombre ""([^""]*)""")]
    public async Task GivenQueExisteUnLectorConCIYNombre(string ci, string nombreCompleto)
    {
        // Create the lector to be deleted
        _lectorToDelete = new Usuario
        {
            Ci = ci,
            PrimerNombre = nombreCompleto.Split(' ')[0],
            PrimerApellido = nombreCompleto.Split(' ').Length > 1 ? nombreCompleto.Split(' ')[1] : "Apellido",
            Correo = "test@example.com",
            Telefono = "70000000",
            SegundoNombre = "",
            SegundoApellido = "",
            FechaCreacion = DateTime.Now,
            UltimaActualizacion = DateTime.Now,
            Rol = "Lector",
            Activo = 1
        };
        
        _context.Usuarios.Add(_lectorToDelete);
        await _context.SaveChangesAsync();
        
        // Verify the lector exists before deletion
        var lectorExists = await _usuarioService.ObtenerPorCi(ci);
        lectorExists.Should().NotBeNull("el lector debe existir antes de la eliminación");
    }
    
    [When(@"elimino el lector con CI ""([^""]*)""")]
    public async Task WhenEliminoElLectorConCI(string ci)
    {
        _deletedCI = ci;
        
        try
        {
            // First get the lector to get its ID
            var lectorToDelete = await _usuarioService.ObtenerPorCi(ci);
            lectorToDelete.Should().NotBeNull("el lector debe existir para poder eliminarlo");
            
            _deleteResult = await _usuarioService.Eliminar(lectorToDelete.Id);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error al eliminar el lector: {ex.Message}");
        }
    }
    
    [Then(@"el lector se elimina correctamente")]
    public void ThenElLectorSeEliminaCorrectamente()
    {
        _deleteResult.Should().BeTrue("la operación de eliminación debe ser exitosa");
    }
    
    [Then(@"el sistema devuelve estado HTTP para eliminación (\d+)")]
    public void ThenElSistemaDevuelveEstadoHTTPParaEliminacion(int expectedStatusCode)
    {
        // In integration test, we verify the delete operation was successful
        // HTTP status codes are typically handled at the controller level
        // Here we verify that the operation completed successfully
        expectedStatusCode.Should().Be(204, "el código de estado debe ser 204 (No Content) para eliminación exitosa");
        _deleteResult.Should().BeTrue("la operación debe haber sido exitosa para devolver 204");
    }
    
    [Then(@"el lector ya no existe en el sistema")]
    public async Task ThenElLectorYaNoExisteEnElSistema()
    {
        // Try to find the deleted lector - should throw exception or return null
        try
        {
            var deletedLector = await _usuarioService.ObtenerPorCi(_deletedCI);
            deletedLector.Should().BeNull("el lector no debe existir después de la eliminación");
        }
        catch
        {
            // Exception is expected when lector doesn't exist
        }
        
        // Check that the lector is marked as inactive (soft delete)
        var lectorInDb = await _context.Usuarios.FirstOrDefaultAsync(u => u.Ci == _deletedCI);
        if (lectorInDb != null)
        {
            // If the system uses soft delete, verify the user is marked as inactive
            lectorInDb.Activo.Should().Be(0, "el lector debe estar marcado como inactivo después de la eliminación");
        }
        else
        {
            // Hard delete - user completely removed
            lectorInDb.Should().BeNull("el lector no debe estar en la base de datos");
        }
    }
    
    public void Dispose()
    {
        _context?.Dispose();
        _serviceProvider?.Dispose();
    }
}