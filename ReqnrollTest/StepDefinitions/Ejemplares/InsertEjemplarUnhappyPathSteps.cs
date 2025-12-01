using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
using GestionBiblioteca.Services.Ejemplar;
using GestionBiblioteca.Services.Libro;
using GestionBiblioteca.Repository;
using Reqnroll;

namespace ReqnrollTest.StepDefinitions.Ejemplares;

[Binding]
public class InsertEjemplarUnhappyPathSteps
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IEjemplarService _ejemplarService;
    private readonly ILibroService _libroService;
    private Ejemplar _ejemplarInput = new();
    private Ejemplar? _ejemplarResult;
    private Exception? _capturedException;
    private Libro? _testLibro;
    private List<ValidationResult> _validationErrors = new();

    public InsertEjemplarUnhappyPathSteps()
    {
        _serviceProvider = BuildServiceProvider();
        _ejemplarService = _serviceProvider.GetRequiredService<IEjemplarService>();
        _libroService = _serviceProvider.GetRequiredService<ILibroService>();
    }

    private static ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        
        services.AddDbContext<MyDbContext>(options =>
            options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                   .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning)));

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<IRepositoryFactory, RepositoryFactory>();
        services.AddScoped<IEjemplarService, EjemplarService>();
        services.AddScoped<ILibroService, LibroService>();

        return services.BuildServiceProvider();
    }

    [Given(@"que tengo acceso al sistema de gestión de biblioteca para ejemplares insert unhappy")]
    public void GivenIHaveAccessToTheBibliotecaManagementSystemParaEjemplaresInsertUnhappy()
    {
        _ejemplarService.Should().NotBeNull();
        _libroService.Should().NotBeNull();
    }

    [Given(@"que existen libros disponibles en el catálogo para ejemplares insert unhappy")]
    public async Task GivenThereAreExistingBooksAvailableInTheCatalogParaEjemplaresInsertUnhappy()
    {
        // Create a test book to use as reference
        _testLibro = await _libroService.Crear(new Libro
        {
            Titulo = "Test Book for Ejemplares Unhappy",
            Isbn = "TEST-EJEMPLARES-UNHAPPY-123",
            Sinopsis = "Test book for ejemplares unhappy testing"
        });
        
        _testLibro.Should().NotBeNull();
        _testLibro.Id.Should().BeGreaterThan(0);
    }

    [Given(@"que quiero crear un nuevo ejemplar para ejemplares insert unhappy")]
    public void GivenIWantToCreateANewEjemplarParaEjemplaresInsertUnhappy()
    {
        _ejemplarInput = new Ejemplar();
        _capturedException = null;
        _ejemplarResult = null;
        _validationErrors.Clear();
    }

    [When(@"proporciono los siguientes datos inválidos del ejemplar para ejemplares insert unhappy:")]
    public void WhenIProvideTheFollowingInvalidEjemplarDataParaEjemplaresInsertUnhappy(Table table)
    {
        foreach (var row in table.Rows)
        {
            var field = row["Campo"];
            var value = row["Valor"];

            switch (field)
            {
                case "IdLibro":
                    _ejemplarInput.IdLibro = value == "null" ? null : 
                        (int.TryParse(value, out var idLibro) ? idLibro : _testLibro?.Id);
                    break;
                case "Descripcion":
                    _ejemplarInput.Descripcion = value == "null" ? null : value;
                    break;
                case "Observaciones":
                    _ejemplarInput.Observaciones = value == "null" ? null : value;
                    break;
                case "Disponible":
                    _ejemplarInput.Disponible = bool.TryParse(value, out var disponible) ? disponible : true;
                    break;
                case "FechaAdquisicion":
                    if (value == "null")
                    {
                        _ejemplarInput.FechaAdquisicion = default(DateTime);
                    }
                    else
                    {
                        _ejemplarInput.FechaAdquisicion = DateTime.TryParse(value, out var fecha) ? fecha : DateTime.Now;
                    }
                    break;
            }
        }
    }

    [When(@"envío el formulario de creación del ejemplar para ejemplares insert unhappy")]
    public async Task WhenISubmitTheEjemplarCreationFormParaEjemplaresInsertUnhappy()
    {
        try
        {
            // First validate the entity
            var context = new ValidationContext(_ejemplarInput);
            var isValid = Validator.TryValidateObject(_ejemplarInput, context, _validationErrors, true);
            
            if (!isValid)
            {
                throw new ValidationException(_validationErrors.First().ErrorMessage);
            }

            _ejemplarResult = await _ejemplarService.Crear(_ejemplarInput);
        }
        catch (Exception ex)
        {
            _capturedException = ex;
        }
    }

    [Then(@"la creación del ejemplar debe fallar para ejemplares insert unhappy")]
    public void ThenTheEjemplarCreationShouldFailParaEjemplaresInsertUnhappy()
    {
        _capturedException.Should().NotBeNull();
        _ejemplarResult.Should().BeNull();
    }

    [Then(@"debo recibir un mensaje de error de validación para ejemplares insert unhappy: ""(.*)""")]
    public void ThenDeboRecibirUnMensajeDeErrorDeValidacionParaEjemplaresInsertUnhappy(string expectedError)
    {
        _capturedException.Should().NotBeNull();
        _capturedException!.Message.Should().Contain(expectedError);
    }
}