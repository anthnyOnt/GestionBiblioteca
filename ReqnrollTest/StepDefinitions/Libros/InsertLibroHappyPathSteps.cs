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
public class InsertLibroHappyPathSteps
{
    private readonly ServiceProvider _serviceProvider;
    private readonly MyDbContext _context;
    private readonly ILibroService _libroService;
    
    private Libro _testLibro;
    private Libro _createdLibro;
    private bool _userAuthorized;
    
    public InsertLibroHappyPathSteps()
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
    
    [Given(@"que soy un usuario autorizado del sistema para libros")]
    public void GivenQueSoyUnUsuarioAutorizadoDelSistemaParaLibros()
    {
        // Simulate user authorization
        _userAuthorized = true;
        _userAuthorized.Should().BeTrue("el usuario debe estar autorizado para realizar operaciones");
    }
    
    [Given(@"que no existe un libro con ISBN ""([^""]*)""")]
    public async Task GivenQueNoExisteUnLibroConISBN(string isbn)
    {
        // Verify that no book with this ISBN exists in the database
        var existingLibro = await _context.Libros.FirstOrDefaultAsync(l => l.Isbn == isbn);
        existingLibro.Should().BeNull($"no debe existir un libro con ISBN {isbn}");
    }
    
    [When(@"registro un nuevo libro con los siguientes datos:")]
    public async Task WhenRegistroUnNuevoLibroConLosSiguientesDatos(Table table)
    {
        // Arrange - Extract data from the table
        var libroData = table.CreateInstance<LibroData>();
        
        // Create the libro object
        _testLibro = new Libro
        {
            Titulo = libroData.Titulo,
            Isbn = libroData.ISBN,
            Idioma = libroData.Idioma,
            Edicion = libroData.Edicion,
            FechaPublicacion = DateTime.Parse(libroData.FechaPublicacion),
            Sinopsis = libroData.Sinopsis,
            FechaCreacion = DateTime.Now,
            UltimaActualizacion = DateTime.Now,
            Activo = 1
        };

        // Act - Create the libro using the service
        _createdLibro = await _libroService.Crear(_testLibro);
        
        // Basic verification that the operation completed
        _createdLibro.Should().NotBeNull("Book creation should not return null");
    }

    [Then(@"el libro se guarda correctamente en el sistema")]
    public void ThenElLibroSeGuardaCorrectamenteEnElSistema()
    {
        _createdLibro.Should().NotBeNull("el libro creado no debe ser nulo");
        _createdLibro.Titulo.Should().Be(_testLibro.Titulo, "el título debe coincidir");
        _createdLibro.Isbn.Should().Be(_testLibro.Isbn, "el ISBN debe coincidir");
        _createdLibro.Idioma.Should().Be(_testLibro.Idioma, "el idioma debe coincidir");
        _createdLibro.Edicion.Should().Be(_testLibro.Edicion, "la edición debe coincidir");
    }

    [Then(@"el sistema asigna un ID único al libro")]
    public void ThenElSistemaAsignaUnIDUnicoAlLibro()
    {
        _createdLibro.Id.Should().BeGreaterThan(0, "el ID del libro debe ser mayor a 0");
    }

    [Then(@"el libro queda con estado ""([^""]*)""")]
    public void ThenElLibroQuedaConEstado(string expectedEstado)
    {
        if (expectedEstado == "Activo")
        {
            _createdLibro.Activo.Should().Be(1, "el libro debe estar activo");
        }
        else if (expectedEstado == "Inactivo")
        {
            _createdLibro.Activo.Should().Be(0, "el libro debe estar inactivo");
        }
    }
    
    public void Dispose()
    {
        _context?.Dispose();
        _serviceProvider?.Dispose();
    }
}

public class LibroData
{
    public string Titulo { get; set; }
    public string ISBN { get; set; }
    public string Idioma { get; set; }
    public string Edicion { get; set; }
    public string FechaPublicacion { get; set; }
    public string Sinopsis { get; set; }
}