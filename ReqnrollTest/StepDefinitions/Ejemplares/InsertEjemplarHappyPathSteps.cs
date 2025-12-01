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
using GestionBiblioteca.Services.Ejemplar;
using GestionBiblioteca.Services.Libro;
using GestionBiblioteca.Repository;
using Reqnroll;

namespace ReqnrollTest.StepDefinitions.Ejemplares;

[Binding]
public class InsertEjemplarHappyPathSteps
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IEjemplarService _ejemplarService;
    private readonly ILibroService _libroService;
    private Ejemplar _ejemplarInput = new();
    private Ejemplar? _ejemplarResult;
    private Exception? _capturedException;
    private Libro? _testLibro;

    public InsertEjemplarHappyPathSteps()
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

    [Given(@"que tengo acceso al sistema de gestión de biblioteca para ejemplares insert")]
    public void GivenIHaveAccessToTheBibliotecaManagementSystemParaEjemplaresInsert()
    {
        _ejemplarService.Should().NotBeNull();
        _libroService.Should().NotBeNull();
    }

    [Given(@"que existen libros disponibles en el catálogo para ejemplares insert")]
    public async Task GivenThereAreExistingBooksAvailableInTheCatalogParaEjemplaresInsert()
    {
        _testLibro = await _libroService.Crear(new Libro
        {
            Titulo = "Test Book for Ejemplares Insert",
            Isbn = "TEST-EJEMPLARES-INSERT-123",
            Sinopsis = "Test book for ejemplares insert testing"
        });
        
        _testLibro.Should().NotBeNull();
        _testLibro.Id.Should().BeGreaterThan(0);
    }

    [Given(@"que quiero crear un nuevo ejemplar para ejemplares insert")]
    public void GivenIWantToCreateANewEjemplarParaEjemplaresInsert()
    {
        _ejemplarInput = new Ejemplar();
        _capturedException = null;
        _ejemplarResult = null;
    }

    [When(@"proporciono los siguientes datos válidos del ejemplar para ejemplares insert:")]
    public void WhenIProvideTheFollowingValidEjemplarDataParaEjemplaresInsert(Table table)
    {
        foreach (var row in table.Rows)
        {
            var field = row["Campo"];
            var value = row["Valor"];

            switch (field)
            {
                case "IdLibro":
                    _ejemplarInput.IdLibro = int.TryParse(value, out var idLibro) ? 
                        (_testLibro?.Id ?? idLibro) : _testLibro?.Id;
                    break;
                case "Descripcion":
                    _ejemplarInput.Descripcion = value;
                    break;
                case "Observaciones":
                    _ejemplarInput.Observaciones = value;
                    break;
                case "Disponible":
                    _ejemplarInput.Disponible = bool.TryParse(value, out var disponible) ? disponible : true;
                    break;
                case "FechaAdquisicion":
                    _ejemplarInput.FechaAdquisicion = DateTime.TryParse(value, out var fecha) ? fecha : DateTime.Now;
                    break;
            }
        }
    }

    [When(@"envío el formulario de creación del ejemplar para ejemplares insert")]
    public async Task WhenISubmitTheEjemplarCreationFormParaEjemplaresInsert()
    {
        try
        {
            _ejemplarResult = await _ejemplarService.Crear(_ejemplarInput);
        }
        catch (Exception ex)
        {
            _capturedException = ex;
        }
    }

    [Then(@"el ejemplar debe crearse exitosamente en la base de datos para ejemplares insert")]
    public void ThenTheEjemplarShouldBeSuccessfullyCreatedInTheDatabaseParaEjemplaresInsert()
    {
        _capturedException.Should().BeNull();
        _ejemplarResult.Should().NotBeNull();
        _ejemplarResult!.Id.Should().BeGreaterThan(0);
        _ejemplarResult.Descripcion.Should().Be(_ejemplarInput.Descripcion);
        _ejemplarResult.IdLibro.Should().Be(_ejemplarInput.IdLibro);
        _ejemplarResult.Observaciones.Should().Be(_ejemplarInput.Observaciones);
        _ejemplarResult.Disponible.Should().Be(_ejemplarInput.Disponible);
        _ejemplarResult.Activo.Should().Be(1);
    }

    [Then(@"el sistema debe retornar una respuesta exitosa de creación para ejemplares insert")]
    public void ThenTheSystemShouldReturnASuccessfulCreationResponseParaEjemplaresInsert()
    {
        _ejemplarResult.Should().NotBeNull();
        _ejemplarResult!.FechaCreacion.Should().NotBeNull();
        _ejemplarResult.CreadoPor.Should().NotBeNull();
    }
}