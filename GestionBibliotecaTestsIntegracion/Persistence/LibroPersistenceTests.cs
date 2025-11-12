using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using GestionBiblioteca.Entities;
using GestionBibliotecaTestsIntegracion.Config;
using GestionBiblioteca.Services.Libro;
using GestionBiblioteca.Context;
using Microsoft.Extensions.DependencyInjection; // para GetRequiredService

namespace GestionBibliotecaTestsIntegracion.Persistence
{
    public class LibroPersistenceTests
    {
        private readonly IServiceProvider _sp;
        private ILibroService Svc => _sp.GetRequiredService<ILibroService>();
        private MyDbContext Ctx => _sp.GetRequiredService<MyDbContext>();

        public LibroPersistenceTests()
        {
            _sp = ServiceTestHelper.BuildProvider();
        }

        [Fact]
        public async Task PER_001_Insertar_Libro()
        {
            var libro = await Svc.Crear(new Libro { Titulo = "Libro Integración" });
            var ok = await Ctx.Libros.AnyAsync(l => l.Id == libro.Id && l.Titulo == "Libro Integración");
            Assert.True(ok);
        }

        [Fact]
        public async Task PER_002_Actualizar_Libro()
        {
            var l = await Svc.Crear(new Libro { Titulo = "Tmp" });
            l.Titulo = "Tmp-Editado";
            await Svc.Actualizar(l);
            var check = await Ctx.Libros.FindAsync(l.Id);
            Assert.Equal("Tmp-Editado", check!.Titulo);
        }

        [Fact]
        public async Task PER_003_Eliminar_Libro()
        {
            var l = await Svc.Crear(new Libro { Titulo = "ParaEliminar" });
            var okDelete = await Svc.Eliminar(l.Id);
            Assert.True(okDelete);
            var gone = await Ctx.Libros.FindAsync(l.Id);
            Assert.Equal(0, gone!.Activo); // borrado lógico
        }
    }
}
