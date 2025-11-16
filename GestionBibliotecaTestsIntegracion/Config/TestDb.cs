using GestionBiblioteca.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GestionBibliotecaTestsIntegracion.Config
{
    internal static class TestDb
    {
        public static string GetConnectionString()
        {
            var env = Environment.GetEnvironmentVariable("MYSQL_TEST_CONN");
            if (!string.IsNullOrWhiteSpace(env)) return env!;

            // Fallback: leer del appsettings del proyecto web
            var basePath = Directory.GetCurrentDirectory();
            // Intentar encontrar la carpeta del proyecto web relativa al proyecto de pruebas
            var webAppSettings = Path.Combine(basePath, "..", "GestionBiblioteca", "appsettings.json");
            if (!File.Exists(webAppSettings))
            {
                // Intentar tambi�n en el propio directorio
                webAppSettings = Path.Combine(basePath, "appsettings.json");
            }

            var config = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(webAppSettings) ?? basePath)
                .AddJsonFile(Path.GetFileName(webAppSettings), optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var conn = config.GetConnectionString("TestConnection");
            if (string.IsNullOrWhiteSpace(conn))
                throw new InvalidOperationException("No se pudo resolver la cadena de conexi�n para pruebas. Configure MYSQL_TEST_CONN o appsettings.json");

            return conn!;
        }

        public static MyDbContext CreateMySqlContext()
        {
            var conn = GetConnectionString();
            var opts = new DbContextOptionsBuilder<MyDbContext>()
                .UseMySql(conn, ServerVersion.AutoDetect(conn))
                .Options;
            return new MyDbContext(opts);
        }
    }
}
