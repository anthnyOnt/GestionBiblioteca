using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GestionBiblioteca.Services.Ejemplar;
using GestionBiblioteca.Services.Libro;
using GestionBiblioteca.Services.Prestamo;
using GestionBiblioteca.Services.Usuario;
using Microsoft.AspNetCore.Mvc.Testing; // RepositoryFactory (si existe en tu repo)

namespace GestionBibliotecaTestsIntegracion.Config
{
    public class ConfigurationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ConfigurationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }
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

        [Theory]
        [InlineData(typeof(IEjemplarService))]
        [InlineData(typeof(ILibroService))]
        [InlineData(typeof(IPrestamoService))]
        [InlineData(typeof(IUsuarioService))]
        public void CFG_003_Todos_los_servicios_se_resuelven_correctamente(Type serviceType)
        {
            using var scope = _factory.Services.CreateScope();
            var sp = scope.ServiceProvider;

            // Si el servicio no está registrado, GetRequiredService lanzará excepción → la prueba falla
            var service = sp.GetRequiredService(serviceType);

            Assert.NotNull(service);
        }
    }
}
