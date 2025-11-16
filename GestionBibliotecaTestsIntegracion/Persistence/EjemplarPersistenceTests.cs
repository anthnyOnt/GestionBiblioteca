using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using GestionBiblioteca.Entities;
using GestionBibliotecaTestsIntegracion.Config;
using GestionBiblioteca.Services.Ejemplar;
using GestionBiblioteca.Services.Libro;
using GestionBiblioteca.Context;
using Microsoft.Extensions.DependencyInjection;

namespace GestionBibliotecaTestsIntegracion.Persistence
{
    public class EjemplarPersistenceTests
    {
        private readonly IServiceProvider _sp;
        private IEjemplarService EjemplarSvc => _sp.GetRequiredService<IEjemplarService>();
        private ILibroService LibroSvc => _sp.GetRequiredService<ILibroService>();
        private MyDbContext Ctx => _sp.GetRequiredService<MyDbContext>();

        public EjemplarPersistenceTests()
        {
            _sp = ServiceTestHelper.BuildProvider();
        }

        [Fact]
        public async Task PER_004_Insertar_Ejemplar()
        {
            var libro = await LibroSvc.Crear(new Libro { Titulo = "Para Ejemplar" });

            var e = await EjemplarSvc.Crear(new Ejemplar
            {
                IdLibro = libro.Id,
                Descripcion = "Ejemplar Integración",
                Disponible = true,
                FechaAdquisicion = DateTime.Now
            });

            var ok = await Ctx.Ejemplars.AnyAsync(x => x.Id == e.Id && x.Descripcion == "Ejemplar Integración");
            Assert.True(ok);
        }

        [Fact]
        public async Task PER_005_Actualizar_Ejemplar()
        {
            var libro = await LibroSvc.Crear(new Libro { Titulo = "Para Ejemplar2" });
            var e = await EjemplarSvc.Crear(new Ejemplar
            {
                IdLibro = libro.Id,
                Descripcion = "Tmp Ej",
                Disponible = true,
                FechaAdquisicion = DateTime.Now
            });

            e.Descripcion = "Tmp Ej Editado";
            await EjemplarSvc.Actualizar(e);

            var check = await Ctx.Ejemplars.FindAsync(e.Id);
            Assert.Equal("Tmp Ej Editado", check!.Descripcion);
        }

        [Fact]
        public async Task PER_006_Eliminar_Ejemplar()
        {
            var libro = await LibroSvc.Crear(new Libro { Titulo = "Para Ejemplar3" });
            var e = await EjemplarSvc.Crear(new Ejemplar
            {
                IdLibro = libro.Id,
                Descripcion = "Ejemplar Borrar",
                Disponible = true,
                FechaAdquisicion = DateTime.Now
            });

            var okDelete = await EjemplarSvc.Eliminar(e.Id);
            Assert.True(okDelete);

            var gone = await Ctx.Ejemplars.FindAsync(e.Id);
            Assert.Equal(0, gone!.Activo); // borrado lógico
        }
    }
}
