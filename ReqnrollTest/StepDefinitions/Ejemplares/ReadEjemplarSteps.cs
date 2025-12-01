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
public class ReadEjemplarSteps
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IEjemplarService _ejemplarService;
    private readonly ILibroService _libroService;
    private Ejemplar? _existingEjemplar;
    private List<Ejemplar>? _searchResult;
    private Exception? _capturedException;
    private Libro? _testLibro;

    public ReadEjemplarSteps()
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

    [Given(@"que tengo acceso al sistema de gestión de biblioteca para ejemplares read")]
    public void GivenIHaveAccessToTheBibliotecaManagementSystemParaEjemplaresRead()
    {
        _ejemplarService.Should().NotBeNull();
        _libroService.Should().NotBeNull();
    }

    [Given(@"que existen ejemplares almacenados en el sistema para ejemplares read")]
    public async Task GivenThereAreExistingEjemplaresStoredInTheSystemParaEjemplaresRead()
    {
        // Create a test book first
        _testLibro = await _libroService.Crear(new Libro
        {
            Titulo = "Test Book for Read Ejemplares",
            Isbn = "TEST-READ-EJEMPLARES-123",
            Sinopsis = "Test book for read ejemplares testing"
        });
        
        _testLibro.Should().NotBeNull();
    }

    [Given(@"que existe un ejemplar con datos válidos para ejemplares read")]
    public async Task GivenThereIsAnExistingEjemplarWithValidDataParaEjemplaresRead()
    {
        _existingEjemplar = await _ejemplarService.Crear(new Ejemplar
        {
            IdLibro = _testLibro!.Id,
            Descripcion = "Ejemplar para Lectura Test",
            Observaciones = "Para pruebas de lectura",
            Disponible = true,
            FechaAdquisicion = DateTime.Now.AddDays(-30)
        });
        
        _existingEjemplar.Should().NotBeNull();
        _existingEjemplar.Id.Should().BeGreaterThan(0);
    }

    [When(@"solicito la información del ejemplar por ID para ejemplares read: (\d+)")]
    public async Task WhenSolicitoLaInformacionDelEjemplarPorIDParaEjemplaresRead(int ejemplarId)
    {
        try
        {
            var actualId = ejemplarId == 1 ? _existingEjemplar!.Id : ejemplarId;
            _searchResult = await _ejemplarService.ObtenerSeleccionados(new List<int> { actualId });
        }
        catch (Exception ex)
        {
            _capturedException = ex;
        }
    }

    [Then(@"debo recibir los detalles completos del ejemplar para ejemplares read")]
    public void ThenIShouldReceiveTheCompleteEjemplarDetailsParaEjemplaresRead()
    {
        _capturedException.Should().BeNull();
        _searchResult.Should().NotBeNull();
        _searchResult.Should().HaveCount(1);
        
        var retrievedEjemplar = _searchResult!.First();
        retrievedEjemplar.Should().NotBeNull();
        retrievedEjemplar.Id.Should().Be(_existingEjemplar!.Id);
    }

    [Then(@"los datos del ejemplar retornados deben coincidir con la información almacenada para ejemplares read")]
    public void ThenTheReturnedEjemplarDataShouldMatchTheStoredInformationParaEjemplaresRead()
    {
        var retrievedEjemplar = _searchResult!.First();
        
        retrievedEjemplar.IdLibro.Should().Be(_existingEjemplar!.IdLibro);
        retrievedEjemplar.Descripcion.Should().Be(_existingEjemplar.Descripcion);
        retrievedEjemplar.Observaciones.Should().Be(_existingEjemplar.Observaciones);
        retrievedEjemplar.Disponible.Should().Be(_existingEjemplar.Disponible);
        retrievedEjemplar.Activo.Should().Be(1);
    }
}