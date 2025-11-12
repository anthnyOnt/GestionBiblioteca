using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore; // agregado para UseMySql y ServerVersion
using Xunit;
using GestionBiblioteca.Context;
using GestionBiblioteca.Entities;
using GestionBiblioteca.Services.Prestamo;
using GestionBiblioteca.Services.Usuario;
using GestionBiblioteca.Services.Libro;
using GestionBiblioteca.Services.Ejemplar;
using GestionBibliotecaTestsIntegracion.Config;

namespace GestionBibliotecaTestsIntegracion.Process
{
    public class PrestamoScenarioTests
    {
        private readonly ServiceProvider _sp;
        private MyDbContext Ctx => _sp.GetRequiredService<MyDbContext>();
        private IPrestamoService PrestamoSvc => _sp.GetRequiredService<IPrestamoService>();
        private IUsuarioService UsuarioSvc => _sp.GetRequiredService<IUsuarioService>();
        private ILibroService LibroSvc => _sp.GetRequiredService<ILibroService>();
        private IEjemplarService EjemplarSvc => _sp.GetRequiredService<IEjemplarService>();

        public PrestamoScenarioTests()
        {
            // Construir un provider con nuestros servicios reales
            var services = new ServiceCollection();
            var conn = TestDb.GetConnectionString();
            services.AddDbContext<MyDbContext>(o => o.UseMySql(conn, ServerVersion.AutoDetect(conn)));
            services.AddHttpContextAccessor();
            services.AddScoped(typeof(GestionBiblioteca.Repository.IRepository<>), typeof(GestionBiblioteca.Repository.Repository<>));
            services.AddScoped<GestionBiblioteca.Repository.IRepositoryFactory, GestionBiblioteca.Repository.RepositoryFactory>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<ILibroService, LibroService>();
            services.AddScoped<IEjemplarService, EjemplarService>();
            services.AddScoped<IPrestamoService, PrestamoService>();
            _sp = services.BuildServiceProvider();
        }

        [Fact]
        public async Task PRO_001_CrearPrestamo_ConUnEjemplar_DisminuyeDisponibilidad()
        {
            // Arrange con servicios
            var usuario = await UsuarioSvc.AgregarLector(new Usuario { PrimerNombre = "Int", PrimerApellido = "Proc", Correo = "proc@test.local", Contrasenia = "pwd" });
            var libro = await LibroSvc.Crear(new Libro { Titulo = "Libro Proc" });
            var ej = await EjemplarSvc.Crear(new Ejemplar { IdLibro = libro.Id, Descripcion = "Ej Proc", Disponible = true, FechaAdquisicion = DateTime.Now });

            // Act
            var items = new List<PrestamoEjemplar>
            {
                new PrestamoEjemplar { IdEjemplar = ej.Id, FechaLimite = DateTime.Today.AddDays(7) }
            };

            await PrestamoSvc.Crear(usuario.Id, items);

            // Assert: ejemplar no disponible
            var reloaded = await Ctx.Ejemplars.FindAsync(ej.Id);
            Xunit.Assert.False(reloaded!.Disponible == true);
        }

        [Fact]
        public async Task PRO_011_CrearPrestamo_EjemplarNoDisponible_Falla()
        {
            var usuario = await UsuarioSvc.AgregarLector(new Usuario { PrimerNombre = "Int", PrimerApellido = "Proc2", Correo = "proc2@test.local", Contrasenia = "pwd" });
            var libro = await LibroSvc.Crear(new Libro { Titulo = "Libro Proc2" });
            var ej = await EjemplarSvc.Crear(new Ejemplar { IdLibro = libro.Id, Descripcion = "Ej NoDisp", Disponible = false, FechaAdquisicion = DateTime.Now });

            var items = new List<PrestamoEjemplar>
            {
                new PrestamoEjemplar { IdEjemplar = ej.Id, FechaLimite = DateTime.Today.AddDays(7) }
            };

            await Xunit.Assert.ThrowsAsync<InvalidOperationException>(() => PrestamoSvc.Crear(usuario.Id, items));
        }
    }
}
