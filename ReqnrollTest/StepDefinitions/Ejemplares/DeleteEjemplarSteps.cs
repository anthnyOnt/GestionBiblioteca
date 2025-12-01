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
public class DeleteEjemplarSteps
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IEjemplarService _ejemplarService;
    private readonly ILibroService _libroService;
    private readonly MyDbContext _dbContext;
    private Ejemplar? _existingEjemplar;
    private bool? _deleteResult;
    private Exception? _capturedException;
    private Libro? _testLibro;

    public DeleteEjemplarSteps()
    {
        _serviceProvider = BuildServiceProvider();
        _ejemplarService = _serviceProvider.GetRequiredService<IEjemplarService>();
        _libroService = _serviceProvider.GetRequiredService<ILibroService>();
        _dbContext = _serviceProvider.GetRequiredService<MyDbContext>();
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

    [Given(@"que tengo acceso al sistema de gestión de biblioteca para ejemplares delete")]
    public void GivenIHaveAccessToTheBibliotecaManagementSystemParaEjemplaresDelete()
    {
        _ejemplarService.Should().NotBeNull();
        _libroService.Should().NotBeNull();
        _dbContext.Should().NotBeNull();
    }

    [Given(@"que existen ejemplares que pueden ser eliminados")]
    public async Task GivenThereAreExistingEjemplaresThatCanBeRemoved()
    {
        // Create a test book first
        _testLibro = await _libroService.Crear(new Libro
        {
            Titulo = "Test Book for Delete Ejemplares",
            Isbn = "TEST-DELETE-EJEMPLARES-123",
            Sinopsis = "Test book for delete ejemplares testing"
        });
        
        _testLibro.Should().NotBeNull();
    }

    [Given(@"que existe un ejemplar que puede ser eliminado para ejemplares delete")]
    public async Task GivenThereIsAnExistingEjemplarThatCanBeDeletedParaEjemplaresDelete()
    {
        _existingEjemplar = await _ejemplarService.Crear(new Ejemplar
        {
            IdLibro = _testLibro!.Id,
            Descripcion = "Ejemplar para Eliminar",
            Observaciones = "Para ser eliminado",
            Disponible = true,
            FechaAdquisicion = DateTime.Now.AddDays(-90)
        });
        
        _existingEjemplar.Should().NotBeNull();
        _existingEjemplar.Id.Should().BeGreaterThan(0);
        _existingEjemplar.Activo.Should().Be(1);
    }

    [When(@"solicito eliminar el ejemplar por ID para ejemplares delete: (\d+)")]
    public async Task WhenSolicitoEliminarElEjemplarPorIDParaEjemplaresDelete(int ejemplarId)
    {
        try
        {
            var actualId = ejemplarId == 1 ? _existingEjemplar!.Id : ejemplarId;
            _deleteResult = await _ejemplarService.Eliminar(actualId);
        }
        catch (Exception ex)
        {
            _capturedException = ex;
        }
    }

    [Then(@"el ejemplar debe eliminarse exitosamente para ejemplares delete")]
    public void ThenTheEjemplarShouldBeSuccessfullyDeletedParaEjemplaresDelete()
    {
        _capturedException.Should().BeNull();
        _deleteResult.Should().NotBeNull();
        _deleteResult.Should().BeTrue();
    }

    [Then(@"el sistema debe retornar una respuesta exitosa de eliminación para ejemplares delete")]
    public void ThenTheSystemShouldReturnASuccessfulDeletionResponseParaEjemplaresDelete()
    {
        _deleteResult.Should().BeTrue();
    }

    [Then(@"el ejemplar debe marcarse como inactivo en la base de datos para ejemplares delete")]
    public async Task ThenTheEjemplarShouldBeMarkedAsInactiveInTheDatabaseParaEjemplaresDelete()
    {
        var deletedEjemplar = await _dbContext.Ejemplars.FindAsync(_existingEjemplar!.Id);
        deletedEjemplar.Should().NotBeNull();
        deletedEjemplar!.Activo.Should().Be(0); // Soft delete - marked as inactive
    }
}