using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using GestionBiblioteca.Context;        // MyDbContext
using GestionBiblioteca.Repository;     // RepositoryFactory (si existe en tu repo)

namespace GestionBibliotecaTestsIntegracion.Config
{
    public class ConfigurationTests
    {
        [Fact]
        public void CFG_001_CargaDeConfiguracion_DefaultConnection_Presente()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var conn = config.GetConnectionString("DefaultConnection");
            Assert.False(string.IsNullOrWhiteSpace(conn));
        }

        [Fact]
        public void CFG_002_TargetFrameworkYProveedorMySql()
        {
            var tfm = Assembly.Load("GestionBiblioteca")
                .GetCustomAttribute<System.Runtime.Versioning.TargetFrameworkAttribute>()?
                .FrameworkName;
            // .NET returns something like ".NETCoreApp,Version=v9.0"
            Assert.Contains("9.0", tfm ?? string.Empty);
            Assert.Contains(".NETCoreApp", tfm ?? string.Empty);

            var pomelo = Assembly.Load("Pomelo.EntityFrameworkCore.MySql");
            Assert.NotNull(pomelo);
        }

        [Fact]
        public void CFG_003_Contenedor_DI_Resuelve_DbContext_y_FabricaRepo()
        {
            var services = new ServiceCollection();
            services.AddDbContext<MyDbContext>(o => o.UseInMemoryDatabase("cfg_check"));

            services.AddScoped<IRepositoryFactory, RepositoryFactory>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            using var sp = services.BuildServiceProvider();
            Assert.NotNull(sp.GetRequiredService<MyDbContext>());
            Assert.NotNull(sp.GetRequiredService<IRepositoryFactory>());
        }
    }
}
