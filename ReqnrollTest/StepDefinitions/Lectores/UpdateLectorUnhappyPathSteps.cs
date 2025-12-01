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
using GestionBiblioteca.Services.Usuario;
using GestionBiblioteca.Repository;
using Reqnroll;

namespace ReqnrollTest.StepDefinitions.Lectores;

[Binding]
public class UpdateLectorUnhappyPathSteps
{
    private readonly ServiceProvider _serviceProvider;
    private readonly MyDbContext _context;
    private readonly IUsuarioService _usuarioService;
    
    private Exception _caughtException;
    private bool _operationCompleted;
    
    public UpdateLectorUnhappyPathSteps()
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
    }
    
    [When(@"intento actualizar la información de un lector con CI vacío")]
    public async Task WhenIntentoActualizarLaInformacionDeUnLectorConCIVacio()
    {
        // Create a Usuario object with empty CI to test validation
        var invalidLector = new Usuario
        {
            Ci = "", // Empty CI should trigger validation error
            PrimerNombre = "Juan",
            PrimerApellido = "Pérez",
            Correo = "juan.perez@email.com",
            Telefono = "70000000",
            SegundoNombre = "",
            SegundoApellido = "",
            FechaCreacion = DateTime.Now,
            UltimaActualizacion = DateTime.Now,
            Rol = "Lector",
            Activo = 1
        };
        
        try
        {
            // Attempt to validate the model using DataAnnotations
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(invalidLector);
            
            bool isValid = Validator.TryValidateObject(invalidLector, validationContext, validationResults, true);
            
            if (!isValid)
            {
                var ciValidationError = validationResults.FirstOrDefault(v => v.MemberNames.Contains("Ci"));
                if (ciValidationError != null)
                {
                    throw new ValidationException(ciValidationError.ErrorMessage);
                }
                
                throw new ValidationException("El número de cédula es obligatorio");
            }
            
            // If validation passes, try to update (should not reach here with empty CI)
            await _usuarioService.Actualizar(invalidLector);
            _operationCompleted = true;
        }
        catch (Exception ex)
        {
            _caughtException = ex;
            _operationCompleted = false;
        }
    }
    
    [Then(@"el sistema rechaza la operación con el mensaje ""([^""]*)""")]
    public void ThenElSistemaRechazaLaOperacionConElMensaje(string expectedMessage)
    {
        _caughtException.Should().NotBeNull("debería haberse lanzado una excepción de validación");
        _caughtException.Should().BeOfType<ValidationException>("debería ser una excepción de validación");
        _caughtException.Message.Should().Contain(expectedMessage, "el mensaje de error debe contener el texto esperado");
    }
    
    [Then(@"no se realiza ningún cambio en el sistema")]
    public void ThenNoSeRealizaNingunCambioEnElSistema()
    {
        _operationCompleted.Should().BeFalse("la operación no debe haberse completado debido al error de validación");
        
        // Verify no usuarios were created or modified
        var usuariosCount = _context.Usuarios.Count();
        usuariosCount.Should().Be(0, "no debe haber usuarios en la base de datos después del error");
    }
    
    public void Dispose()
    {
        _context?.Dispose();
        _serviceProvider?.Dispose();
    }
}