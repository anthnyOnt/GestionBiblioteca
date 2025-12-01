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
public class EjemplarSharedSteps
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IEjemplarService _ejemplarService;
    private readonly ILibroService _libroService;

    public EjemplarSharedSteps()
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

    public IServiceProvider ServiceProvider => _serviceProvider;
    public IEjemplarService EjemplarService => _ejemplarService;
    public ILibroService LibroService => _libroService;
}