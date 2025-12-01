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
using GestionBiblioteca.Services.Usuario;
using GestionBiblioteca.Repository;
using Reqnroll;
using Reqnroll.Assist;
using System.ComponentModel.DataAnnotations;

namespace ReqnrollTest.StepDefinitions.Lectores;

[Binding]
public class InsertLectorUnhappyPathSteps
{
    private readonly ServiceProvider _serviceProvider;
    private readonly MyDbContext _context;
    private readonly IUsuarioService _usuarioService;
    
    private Usuario _testLector;
    private Exception _caughtException;
    private string _validationErrorMessage;
    
    public InsertLectorUnhappyPathSteps()
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
        services.AddScoped<IUsuarioService, UsuarioService>();
        
        // Add HttpContextAccessor for services that need it
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        
        _serviceProvider = services.BuildServiceProvider();
        _context = _serviceProvider.GetRequiredService<MyDbContext>();
        _usuarioService = _serviceProvider.GetRequiredService<IUsuarioService>();
        
        // Ensure database is created
        _context.Database.EnsureCreated();
    }

    [When(@"intento registrar un lector con los siguientes datos:")]
    public async Task WhenIntentoRegistrarUnLectorConLosSiguientesDatos(Table table)
    {
        try
        {
            // Arrange - Extract data from the table
            var lectorData = table.CreateInstance<LectorDataUnhappy>();
            
            // Create the lector object
            _testLector = new Usuario
            {
                Ci = lectorData.CI,
                PrimerNombre = lectorData.PrimerNombre,
                SegundoNombre = lectorData.SegundoNombre,
                PrimerApellido = lectorData.PrimerApellido,
                SegundoApellido = lectorData.SegundoApellido,
                Telefono = lectorData.Telefono,
                Correo = string.IsNullOrWhiteSpace(lectorData.Correo) ? null : lectorData.Correo
            };

            // Validate the model using data annotations
            var validationContext = new ValidationContext(_testLector);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(_testLector, validationContext, validationResults, true);

            if (!isValid)
            {
                // Get the first validation error message
                var firstError = validationResults.FirstOrDefault();
                if (firstError != null)
                {
                    _validationErrorMessage = firstError.ErrorMessage;
                    throw new ValidationException(_validationErrorMessage);
                }
            }

            // Act - Try to create the lector using the service (this should fail)
            await _usuarioService.AgregarLector(_testLector);
        }
        catch (Exception ex)
        {
            // Capture the exception for verification
            _caughtException = ex;
        }
    }

    [Then(@"el sistema rechaza la operacion con el mensaje ""(.*)""")]
    public void ThenElSistemaRechazaLaOperacionConElMensaje(string mensajeEsperado)
    {
        // Verify that an exception was caught
        _caughtException.Should().NotBeNull("An exception should have been thrown for invalid data");
        
        // Verify the exception is a validation exception
        _caughtException.Should().BeOfType<ValidationException>("Should be a validation exception");
        
        // Verify the error message matches what's expected
        _caughtException.Message.Should().Be(mensajeEsperado, "Error message should match the expected validation message");
    }

    [Then(@"no se crea ningún lector en el sistema")]
    public async Task ThenNoSeCreaNingunLectorEnElSistema()
    {
        // Verify that no lector was created by trying to find it by CI
        if (_testLector?.Ci != null)
        {
            var savedLector = await _usuarioService.ObtenerPorCi(_testLector.Ci);
            savedLector.Should().BeNull("No lector should have been saved to the database due to validation failure");
        }
        
        // Also verify by checking the total count of users in the database
        var allUsers = await _usuarioService.ObtenerTodos();
        allUsers.Should().BeEmpty("No users should exist in the database after failed validation");
    }

    [AfterScenario]
    public void CleanupAfterScenario()
    {
        // Clean up database after each scenario
        try
        {
            _context?.Database.EnsureDeleted();
            _context?.Dispose();
        }
        catch (Exception ex)
        {
            // Log or handle cleanup errors if needed
            Console.WriteLine($"Cleanup warning: {ex.Message}");
        }
        finally
        {
            _serviceProvider?.Dispose();
        }
    }
}

// Helper class for table data binding (unhappy path)
public class LectorDataUnhappy
{
    public string CI { get; set; }
    public string PrimerNombre { get; set; }
    public string SegundoNombre { get; set; }
    public string PrimerApellido { get; set; }
    public string SegundoApellido { get; set; }
    public string Telefono { get; set; }
    public string Correo { get; set; }
}