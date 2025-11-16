using GestionBiblioteca.Context;
using GestionBiblioteca.Repository;
using GestionBiblioteca.Services.Libro;
using GestionBiblioteca.Services.Ejemplar;
using GestionBiblioteca.Services.Usuario;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GestionBibliotecaTestsIntegracion.Config
{
    internal static class ServiceTestHelper
    {
        public static ServiceProvider BuildProvider()
        {
            var services = new ServiceCollection();

            // DbContext real (MySQL) usando TestDb
            var conn = TestDb.GetConnectionString();
            services.AddDbContext<MyDbContext>(o => o.UseMySql(conn, ServerVersion.AutoDetect(conn)));

            // Infraestructura
            services.AddHttpContextAccessor();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IRepositoryFactory, RepositoryFactory>();

            // Servicios dominio
            services.AddScoped<ILibroService, LibroService>();
            services.AddScoped<IEjemplarService, EjemplarService>();
            services.AddScoped<IUsuarioService, UsuarioService>();

            return services.BuildServiceProvider();
        }
    }
}
