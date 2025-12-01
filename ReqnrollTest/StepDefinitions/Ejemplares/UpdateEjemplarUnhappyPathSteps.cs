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
public class UpdateEjemplarUnhappyPathSteps
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IEjemplarService _ejemplarService;
    private readonly ILibroService _libroService;
    private Ejemplar? _existingEjemplar;
    private Ejemplar? _updateResult;
    private Exception? _capturedException;
    private Libro? _testLibro;
    private readonly Dictionary<string, string> _updateData = new();
    private List<ValidationResult> _validationErrors = new();

    public UpdateEjemplarUnhappyPathSteps()
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

    [Given(@"que tengo acceso al sistema de gestión de biblioteca para ejemplares update unhappy")]
    public void GivenIHaveAccessToTheBibliotecaManagementSystemParaEjemplaresUpdateUnhappy()
    {
        _ejemplarService.Should().NotBeNull();
        _libroService.Should().NotBeNull();
    }

    [Given(@"que existen ejemplares que pueden ser modificados para update unhappy")]
    public async Task GivenThereAreExistingEjemplaresThatCanBeModifiedParaUpdateUnhappy()
    {
        // Create a test book first
        _testLibro = await _libroService.Crear(new Libro
        {
            Titulo = "Test Book for Update Ejemplares Unhappy",
            Isbn = "TEST-UPDATE-EJEMPLARES-UNHAPPY-123",
            Sinopsis = "Test book for update ejemplares unhappy testing"
        });
        
        _testLibro.Should().NotBeNull();
    }

    [Given(@"que existe un ejemplar que puede ser actualizado para ejemplares update unhappy")]
    public async Task GivenThereIsAnExistingEjemplarThatCanBeUpdatedParaEjemplaresUpdateUnhappy()
    {
        _existingEjemplar = await _ejemplarService.Crear(new Ejemplar
        {
            IdLibro = _testLibro!.Id,
            Descripcion = "Ejemplar Original Unhappy",
            Observaciones = "Estado original",
            Disponible = true,
            FechaAdquisicion = DateTime.Now.AddDays(-60)
        });
        
        _existingEjemplar.Should().NotBeNull();
        _existingEjemplar.Id.Should().BeGreaterThan(0);
    }

    [When(@"intento actualizar el ejemplar con datos inválidos para ejemplares update unhappy:")]
    public void WhenIAttemptToUpdateTheEjemplarWithInvalidDataParaEjemplaresUpdateUnhappy(Table table)
    {
        _updateData.Clear();
        
        foreach (var row in table.Rows)
        {
            _updateData[row["Campo"]] = row["Valor"];
        }
    }

    [When(@"envío el formulario de actualización del ejemplar para ejemplares update unhappy")]
    public async Task WhenISubmitTheEjemplarUpdateFormParaEjemplaresUpdateUnhappy()
    {
        try
        {
            // Prepare updated ejemplar with invalid data
            var updatedEjemplar = new Ejemplar
            {
                Id = _existingEjemplar!.Id,
                IdLibro = _existingEjemplar.IdLibro,
                FechaAdquisicion = _existingEjemplar.FechaAdquisicion,
                Descripcion = _updateData["Descripcion"] == "null" ? null : _updateData["Descripcion"],
                Observaciones = _updateData.GetValueOrDefault("Observaciones", _existingEjemplar.Observaciones),
                Disponible = _existingEjemplar.Disponible
            };

            // Validate the entity first
            var context = new ValidationContext(updatedEjemplar);
            var isValid = Validator.TryValidateObject(updatedEjemplar, context, _validationErrors, true);
            
            if (!isValid)
            {
                throw new ValidationException(_validationErrors.First().ErrorMessage);
            }

            _updateResult = await _ejemplarService.Actualizar(updatedEjemplar);
        }
        catch (Exception ex)
        {
            _capturedException = ex;
        }
    }

    [Then(@"la actualización del ejemplar debe fallar para ejemplares update unhappy")]
    public void ThenTheEjemplarUpdateShouldFailParaEjemplaresUpdateUnhappy()
    {
        _capturedException.Should().NotBeNull();
        _updateResult.Should().BeNull();
    }

    [Then(@"debo recibir un mensaje de error de validación para ejemplares update unhappy: ""(.*)""")]
    public void ThenDeboRecibirUnMensajeDeErrorDeValidacionParaEjemplaresUpdateUnhappy(string expectedError)
    {
        _capturedException.Should().NotBeNull();
        _capturedException!.Message.Should().Contain(expectedError);
    }
}