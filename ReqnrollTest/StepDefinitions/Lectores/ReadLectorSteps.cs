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

namespace ReqnrollTest.StepDefinitions.Lectores;

[Binding]
public class ReadLectorSteps
{
    private readonly ServiceProvider _serviceProvider;
    private readonly MyDbContext _context;
    private readonly IUsuarioService _usuarioService;
    
    private Usuario _expectedLector;
    private Usuario _retrievedLector;
    
    public ReadLectorSteps()
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
    
    [Given(@"que existe un lector con CI ""([^""]*)"", nombre ""([^""]*)"" y correo ""([^""]*)""")]
    public async Task GivenQueExisteUnLectorConDatos(string ci, string nombreCompleto, string correo)
    {
        // Create the lector with provided data
        _expectedLector = new Usuario
        {
            Ci = ci,
            PrimerNombre = nombreCompleto.Split(' ')[0],
            PrimerApellido = nombreCompleto.Split(' ').Length > 1 ? nombreCompleto.Split(' ')[1] : "Apellido",
            Correo = correo,
            Telefono = "70000000",
            SegundoNombre = "",
            SegundoApellido = "",
            FechaCreacion = DateTime.Now,
            UltimaActualizacion = DateTime.Now,
            Rol = "Lector",
            Activo = 1
        };
        
        _context.Usuarios.Add(_expectedLector);
        await _context.SaveChangesAsync();
    }
    
    [When(@"consulto la información del lector con CI ""([^""]*)""")]
    public async Task WhenConsultoLaInformacionDelLectorConCI(string ci)
    {
        _retrievedLector = await _usuarioService.ObtenerPorCi(ci);
    }
    
    [Then(@"el sistema muestra la información completa del lector:")]
    public void ThenElSistemaMuestraLaInformacionCompletaDelLector(Table table)
    {
        _retrievedLector.Should().NotBeNull("el lector debe existir en el sistema");
        
        var expectedData = table.CreateInstance<LectorExpectedData>();
        
        _retrievedLector.Ci.Should().Be(expectedData.CI, "el CI debe coincidir");
        (_retrievedLector.PrimerNombre + " " + _retrievedLector.PrimerApellido).Should().Be(expectedData.Nombre, "el nombre debe coincidir");
        _retrievedLector.Correo.Should().Be(expectedData.Correo, "el correo debe coincidir");
        _retrievedLector.Activo.Should().Be(1, "el usuario debe estar activo");
    }
    
    public void Dispose()
    {
        _context?.Dispose();
        _serviceProvider?.Dispose();
    }
}

public class LectorExpectedData
{
    public string CI { get; set; }
    public string Nombre { get; set; }
    public string Correo { get; set; }
    public string Estado { get; set; }
}