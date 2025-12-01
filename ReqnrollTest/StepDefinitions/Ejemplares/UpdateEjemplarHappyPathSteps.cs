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
public class UpdateEjemplarHappyPathSteps
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IEjemplarService _ejemplarService;
    private readonly ILibroService _libroService;
    private Ejemplar? _existingEjemplar;
    private Ejemplar? _updateResult;
    private Exception? _capturedException;
    private Libro? _testLibro;
    private readonly Dictionary<string, string> _updateData = new();

    public UpdateEjemplarHappyPathSteps()
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

    [Given(@"que tengo acceso al sistema de gestión de biblioteca para ejemplares update happy")]
    public void GivenIHaveAccessToTheBibliotecaManagementSystemParaEjemplaresUpdateHappy()
    {
        _ejemplarService.Should().NotBeNull();
        _libroService.Should().NotBeNull();
    }

    [Given(@"que existen ejemplares que pueden ser modificados para update happy")]
    public async Task GivenThereAreExistingEjemplaresThatCanBeModifiedParaUpdateHappy()
    {
        // Create a test book first
        _testLibro = await _libroService.Crear(new Libro
        {
            Titulo = "Test Book for Update Ejemplares",
            Isbn = "TEST-UPDATE-EJEMPLARES-123",
            Sinopsis = "Test book for update ejemplares testing"
        });
        
        _testLibro.Should().NotBeNull();
    }

    [Given(@"que existe un ejemplar que puede ser actualizado para ejemplares update happy")]
    public async Task GivenThereIsAnExistingEjemplarThatCanBeUpdatedParaEjemplaresUpdateHappy()
    {
        _existingEjemplar = await _ejemplarService.Crear(new Ejemplar
        {
            IdLibro = _testLibro!.Id,
            Descripcion = "Ejemplar Original",
            Observaciones = "Estado original",
            Disponible = true,
            FechaAdquisicion = DateTime.Now.AddDays(-60)
        });
        
        _existingEjemplar.Should().NotBeNull();
        _existingEjemplar.Id.Should().BeGreaterThan(0);
    }

    [When(@"actualizo el ejemplar con los siguientes datos válidos para ejemplares update happy:")]
    public void WhenIUpdateTheEjemplarWithTheFollowingValidDataParaEjemplaresUpdateHappy(Table table)
    {
        _updateData.Clear();
        
        foreach (var row in table.Rows)
        {
            _updateData[row["Campo"]] = row["Valor"];
        }
    }

    [When(@"envío el formulario de actualización del ejemplar para ejemplares update happy")]
    public async Task WhenISubmitTheEjemplarUpdateFormParaEjemplaresUpdateHappy()
    {
        try
        {
            // Prepare updated ejemplar
            var updatedEjemplar = new Ejemplar
            {
                Id = _existingEjemplar!.Id,
                IdLibro = _existingEjemplar.IdLibro,
                FechaAdquisicion = _existingEjemplar.FechaAdquisicion,
                Descripcion = _updateData.GetValueOrDefault("Descripcion", _existingEjemplar.Descripcion),
                Observaciones = _updateData.GetValueOrDefault("Observaciones", _existingEjemplar.Observaciones),
                Disponible = _updateData.ContainsKey("Disponible") ? 
                    bool.Parse(_updateData["Disponible"]) : _existingEjemplar.Disponible
            };

            _updateResult = await _ejemplarService.Actualizar(updatedEjemplar);
        }
        catch (Exception ex)
        {
            _capturedException = ex;
        }
    }

    [Then(@"el ejemplar debe actualizarse exitosamente en la base de datos para ejemplares update happy")]
    public void ThenTheEjemplarShouldBeSuccessfullyUpdatedInTheDatabaseParaEjemplaresUpdateHappy()
    {
        _capturedException.Should().BeNull();
        _updateResult.Should().NotBeNull();
        _updateResult!.Id.Should().Be(_existingEjemplar!.Id);
        
        // Verify the updated fields
        if (_updateData.ContainsKey("Descripcion"))
        {
            _updateResult.Descripcion.Should().Be(_updateData["Descripcion"]);
        }
        
        if (_updateData.ContainsKey("Observaciones"))
        {
            _updateResult.Observaciones.Should().Be(_updateData["Observaciones"]);
        }
        
        if (_updateData.ContainsKey("Disponible"))
        {
            _updateResult.Disponible.Should().Be(bool.Parse(_updateData["Disponible"]));
        }
    }

    [Then(@"el sistema debe retornar una respuesta exitosa de actualización para ejemplares update happy")]
    public void ThenTheSystemShouldReturnASuccessfulUpdateResponseParaEjemplaresUpdateHappy()
    {
        _updateResult.Should().NotBeNull();
        _updateResult!.UltimaActualizacion.Should().NotBeNull();
        _updateResult.UltimaActualizacion.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMinutes(1));
    }
}