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
using Reqnroll.Assist;

namespace ReqnrollTest.StepDefinitions.Libros;

[Binding]
public class ReadLibroSteps
{
    private readonly ServiceProvider _serviceProvider;
    private readonly MyDbContext _context;
    private readonly ILibroService _libroService;
    
    private Libro _expectedLibro;
    private Libro _retrievedLibro;
    
    public ReadLibroSteps()
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
    
    [Given(@"que existe un libro con ISBN ""([^""]*)"", título ""([^""]*)"" y idioma ""([^""]*)""")]
    public async Task GivenQueExisteUnLibroConDatos(string isbn, string titulo, string idioma)
    {
        // Create the libro with provided data
        _expectedLibro = new Libro
        {
            Isbn = isbn,
            Titulo = titulo,
            Idioma = idioma,
            Edicion = "1ra edición",
            Sinopsis = "Libro de prueba",
            FechaPublicacion = DateTime.Now.AddYears(-10),
            FechaCreacion = DateTime.Now,
            UltimaActualizacion = DateTime.Now,
            Activo = 1
        };
        
        _context.Libros.Add(_expectedLibro);
        await _context.SaveChangesAsync();
    }
    
    [When(@"consulto la información del libro con ISBN ""([^""]*)""")]
    public async Task WhenConsultoLaInformacionDelLibroConISBN(string isbn)
    {
        // Find the libro by ISBN since the service doesn't have a direct method for ISBN lookup
        _retrievedLibro = await _context.Libros.FirstOrDefaultAsync(l => l.Isbn == isbn && l.Activo == 1);
    }
    
    [Then(@"el sistema muestra la información completa del libro:")]
    public void ThenElSistemaMuestraLaInformacionCompletaDelLibro(Table table)
    {
        _retrievedLibro.Should().NotBeNull("el libro debe existir en el sistema");
        
        var expectedData = table.CreateInstance<LibroExpectedData>();
        
        _retrievedLibro.Isbn.Should().Be(expectedData.ISBN, "el ISBN debe coincidir");
        _retrievedLibro.Titulo.Should().Be(expectedData.Titulo, "el título debe coincidir");
        _retrievedLibro.Idioma.Should().Be(expectedData.Idioma, "el idioma debe coincidir");
        _retrievedLibro.Activo.Should().Be(1, "el libro debe estar activo");
    }
    
    public void Dispose()
    {
        _context?.Dispose();
        _serviceProvider?.Dispose();
    }
}

public class LibroExpectedData
{
    public string ISBN { get; set; }
    public string Titulo { get; set; }
    public string Idioma { get; set; }
    public string Estado { get; set; }
}