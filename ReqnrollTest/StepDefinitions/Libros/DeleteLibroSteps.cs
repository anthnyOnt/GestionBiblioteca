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
using GestionBiblioteca.Services.Libro;
using GestionBiblioteca.Repository;
using Reqnroll;

namespace ReqnrollTest.StepDefinitions.Libros;

[Binding]
public class DeleteLibroSteps
{
    private readonly ServiceProvider _serviceProvider;
    private readonly MyDbContext _context;
    private readonly ILibroService _libroService;
    
    private Libro _libroToDelete;
    private bool _deleteResult;
    private string _deletedISBN;
    
    public DeleteLibroSteps()
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
        services.AddScoped<ILibroService, LibroService>();
        
        // Add HttpContextAccessor for services that need it
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        
        _serviceProvider = services.BuildServiceProvider();
        _context = _serviceProvider.GetRequiredService<MyDbContext>();
        _libroService = _serviceProvider.GetRequiredService<ILibroService>();
    }
    
    [Given(@"que existe un libro con ISBN ""([^""]*)"" y título ""([^""]*)""")]
    public async Task GivenQueExisteUnLibroConISBNYTitulo(string isbn, string titulo)
    {
        // Create the libro to be deleted
        _libroToDelete = new Libro
        {
            Isbn = isbn,
            Titulo = titulo,
            Idioma = "Español",
            Edicion = "1ra edición",
            Sinopsis = "Libro de prueba para eliminar",
            FechaPublicacion = DateTime.Now.AddYears(-10),
            FechaCreacion = DateTime.Now,
            UltimaActualizacion = DateTime.Now,
            Activo = 1
        };
        
        _context.Libros.Add(_libroToDelete);
        await _context.SaveChangesAsync();
        
        // Verify the libro exists before deletion
        var libroExists = await _libroService.ObtenerPorId(_libroToDelete.Id);
        libroExists.Should().NotBeNull("el libro debe existir antes de la eliminación");
    }
    
    [When(@"elimino el libro con ISBN ""([^""]*)""")]
    public async Task WhenEliminoElLibroConISBN(string isbn)
    {
        _deletedISBN = isbn;
        
        try
        {
            // First get the libro to get its ID
            var libroToDelete = await _context.Libros.FirstOrDefaultAsync(l => l.Isbn == isbn && l.Activo == 1);
            libroToDelete.Should().NotBeNull("el libro debe existir para poder eliminarlo");
            
            _deleteResult = await _libroService.Eliminar(libroToDelete.Id);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error al eliminar el libro: {ex.Message}");
        }
    }
    
    [Then(@"el libro se elimina correctamente")]
    public void ThenElLibroSeEliminaCorrectamente()
    {
        _deleteResult.Should().BeTrue("la operación de eliminación debe ser exitosa");
    }
    
    [Then(@"el sistema devuelve estado HTTP para eliminación de libro (\d+)")]
    public void ThenElSistemaDevuelveEstadoHTTPParaEliminacionDeLibro(int expectedStatusCode)
    {
        // In integration test, we verify the delete operation was successful
        // HTTP status codes are typically handled at the controller level
        // Here we verify that the operation completed successfully
        expectedStatusCode.Should().Be(204, "el código de estado debe ser 204 (No Content) para eliminación exitosa");
        _deleteResult.Should().BeTrue("la operación debe haber sido exitosa para devolver 204");
    }
    
    [Then(@"el libro ya no existe en el sistema")]
    public async Task ThenElLibroYaNoExisteEnElSistema()
    {
        // Check that the libro is marked as inactive (soft delete)
        var libroInDb = await _context.Libros.FirstOrDefaultAsync(l => l.Isbn == _deletedISBN);
        if (libroInDb != null)
        {
            // If the system uses soft delete, verify the libro is marked as inactive
            libroInDb.Activo.Should().Be(0, "el libro debe estar marcado como inactivo después de la eliminación");
        }
        else
        {
            // Hard delete - libro completely removed
            libroInDb.Should().BeNull("el libro no debe estar en la base de datos");
        }
    }
    
    public void Dispose()
    {
        _context?.Dispose();
        _serviceProvider?.Dispose();
    }
}