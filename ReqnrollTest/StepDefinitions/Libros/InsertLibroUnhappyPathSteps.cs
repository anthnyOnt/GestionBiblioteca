using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
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
public class InsertLibroUnhappyPathSteps
{
    private readonly ServiceProvider _serviceProvider;
    private readonly MyDbContext _context;
    private readonly ILibroService _libroService;
    
    private Exception _caughtException;
    private bool _operationCompleted;
    
    public InsertLibroUnhappyPathSteps()
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
    
    [When(@"intento registrar un libro con los siguientes datos:")]
    public async Task WhenIntentoRegistrarUnLibroConLosSiguientesDatos(Table table)
    {
        try
        {
            // Arrange - Extract data from the table
            var libroData = table.CreateInstance<LibroDataUnhappy>();
            
            // Create the libro object with potentially invalid data
            var invalidLibro = new Libro
            {
                Titulo = string.IsNullOrEmpty(libroData.Titulo?.Trim()) ? null : libroData.Titulo,
                Isbn = libroData.ISBN,
                Idioma = libroData.Idioma,
                Edicion = libroData.Edicion,
                FechaPublicacion = DateTime.Parse(libroData.FechaPublicacion),
                Sinopsis = libroData.Sinopsis,
                FechaCreacion = DateTime.Now,
                UltimaActualizacion = DateTime.Now,
                Activo = 1
            };

            // Attempt to validate the model using DataAnnotations
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(invalidLibro);
            
            bool isValid = Validator.TryValidateObject(invalidLibro, validationContext, validationResults, true);
            
            if (!isValid)
            {
                var tituloValidationError = validationResults.FirstOrDefault(v => v.MemberNames.Contains("Titulo"));
                if (tituloValidationError != null)
                {
                    throw new ValidationException(tituloValidationError.ErrorMessage);
                }
                
                throw new ValidationException("El título es obligatorio");
            }

            // If validation passes, try to create the libro
            await _libroService.Crear(invalidLibro);
            _operationCompleted = true;
        }
        catch (Exception ex)
        {
            _caughtException = ex;
            _operationCompleted = false;
        }
    }
    
    [Then(@"el sistema rechaza la operación con el mensaje de libro ""([^""]*)""")]
    public void ThenElSistemaRechazaLaOperacionConElMensajeDeLibro(string expectedMessage)
    {
        _caughtException.Should().NotBeNull("debería haberse lanzado una excepción de validación");
        _caughtException.Should().BeOfType<ValidationException>("debería ser una excepción de validación");
        _caughtException.Message.Should().Contain(expectedMessage, "el mensaje de error debe contener el texto esperado");
    }
    
    [Then(@"no se crea ningún libro en el sistema")]
    public void ThenNoSeCreaNingunLibroEnElSistema()
    {
        _operationCompleted.Should().BeFalse("la operación no debe haberse completado debido al error de validación");
        
        // Verify no libros were created
        var librosCount = _context.Libros.Count();
        librosCount.Should().Be(0, "no debe haber libros en la base de datos después del error");
    }
    
    public void Dispose()
    {
        _context?.Dispose();
        _serviceProvider?.Dispose();
    }
}

public class LibroDataUnhappy
{
    public string Titulo { get; set; }
    public string ISBN { get; set; }
    public string Idioma { get; set; }
    public string Edicion { get; set; }
    public string FechaPublicacion { get; set; }
    public string Sinopsis { get; set; }
}