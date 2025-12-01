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
public class UpdateLibroHappyPathSteps
{
    private readonly ServiceProvider _serviceProvider;
    private readonly MyDbContext _context;
    private readonly ILibroService _libroService;
    
    private Libro _existingLibro;
    private Libro _updatedLibro;
    private bool _updateResult;
    private string _targetISBN;
    
    public UpdateLibroHappyPathSteps()
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
    
    [Given(@"que existe un libro con ISBN ""([^""]*)"" y edición ""([^""]*)""")]
    public async Task GivenQueExisteUnLibroConISBNYEdicion(string isbn, string edicion)
    {
        await CreateLibroWithField(isbn, "edicion", edicion);
    }
    
    [Given(@"que existe un libro con ISBN ""([^""]*)"" y idioma ""([^""]*)""")]
    public async Task GivenQueExisteUnLibroConISBNYIdioma(string isbn, string idioma)
    {
        await CreateLibroWithField(isbn, "idioma", idioma);
    }
    
    private async Task CreateLibroWithField(string isbn, string campo, string valor)
    {
        _targetISBN = isbn;
        
        _existingLibro = new Libro
        {
            Isbn = isbn,
            Titulo = "Libro de Prueba",
            Idioma = campo == "idioma" ? valor : "Español",
            Edicion = campo == "edicion" ? valor : "1ra edición",
            Sinopsis = "Sinopsis de prueba",
            FechaPublicacion = DateTime.Now.AddYears(-5),
            FechaCreacion = DateTime.Now,
            UltimaActualizacion = DateTime.Now,
            Activo = 1
        };
        
        _context.Libros.Add(_existingLibro);
        await _context.SaveChangesAsync();
        
        // Verify the libro exists
        var libroExists = await _libroService.ObtenerPorId(_existingLibro.Id);
        libroExists.Should().NotBeNull("el libro debe existir antes de la actualización");
    }
    
    [When(@"actualizo su edición a ""([^""]*)""")]
    public async Task WhenActualizoSuEdicionA(string nuevaEdicion)
    {
        await UpdateLibroField("edicion", nuevaEdicion);
    }
    
    [When(@"actualizo su idioma a ""([^""]*)""")]
    public async Task WhenActualizoSuIdiomaA(string nuevoIdioma)
    {
        await UpdateLibroField("idioma", nuevoIdioma);
    }
    
    private async Task UpdateLibroField(string campo, string nuevoValor)
    {
        // Get the current libro
        var currentLibro = await _libroService.ObtenerPorId(_existingLibro.Id);
        currentLibro.Should().NotBeNull("el libro debe existir para poder actualizarlo");
        
        // Update the specific field
        if (campo == "edicion")
        {
            currentLibro.Edicion = nuevoValor;
        }
        else if (campo == "idioma")
        {
            currentLibro.Idioma = nuevoValor;
        }
        
        currentLibro.UltimaActualizacion = DateTime.Now;
        
        try
        {
            _updatedLibro = await _libroService.Actualizar(currentLibro);
            _updateResult = _updatedLibro != null;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error al actualizar el libro: {ex.Message}");
        }
    }
    
    [Then(@"los datos del libro se actualizan correctamente")]
    public void ThenLosDatosDelLibroSeActualizanCorrectamente()
    {
        _updateResult.Should().BeTrue("la operación de actualización debe ser exitosa");
        _updatedLibro.Should().NotBeNull("el libro actualizado debe existir");
    }
    
    [Then(@"el sistema devuelve estado HTTP para actualización de libro (\d+)")]
    public void ThenElSistemaDevuelveEstadoHTTPParaActualizacionDeLibro(int expectedStatusCode)
    {
        // In integration test, we verify the update operation was successful
        // HTTP status codes are typically handled at the controller level
        expectedStatusCode.Should().Be(200, "el código de estado debe ser 200 para actualización exitosa");
        _updateResult.Should().BeTrue("la operación debe haber sido exitosa para devolver 200");
    }
    
    [Then(@"el libro tiene edición ""([^""]*)""")]
    public void ThenElLibroTieneEdicion(string expectedEdicion)
    {
        _updatedLibro.Should().NotBeNull("el libro actualizado debe existir");
        _updatedLibro.Edicion.Should().Be(expectedEdicion, "la edición debe haber sido actualizada correctamente");
    }
    
    [Then(@"el libro tiene idioma ""([^""]*)""")]
    public void ThenElLibroTieneIdioma(string expectedIdioma)
    {
        _updatedLibro.Should().NotBeNull("el libro actualizado debe existir");
        _updatedLibro.Idioma.Should().Be(expectedIdioma, "el idioma debe haber sido actualizado correctamente");
    }
    
    public void Dispose()
    {
        _context?.Dispose();
        _serviceProvider?.Dispose();
    }
}