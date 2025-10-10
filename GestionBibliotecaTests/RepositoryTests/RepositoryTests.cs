using GestionBiblioteca.Context;
using GestionBiblioteca.Repository;
using GestionBiblioteca.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace GestionBiblioteca.Tests
{
    public class RepositoryTests
    {
        private MyDbContextTest CrearContexto()
        {
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new MyDbContextTest(options);
        }

        [Fact]
        public async Task ObtenerTodos_DeberiaRetornarTodasLasEntidades()
        {
            using var context = CrearContexto();
            context.Libros.Add(new Libro { Id = 1, Titulo = "A" });
            context.Libros.Add(new Libro { Id = 2, Titulo = "B" });
            await context.SaveChangesAsync();

            var repo = new Repository<Libro>(context);
            var resultado = await repo.ObtenerTodos();

            Assert.Equal(2, resultado.Count);
        }

        [Fact]
        public void ObtenerPorConsulta_DeberiaPermitirFiltrado()
        {
            using var context = CrearContexto();
            context.Libros.AddRange(
                new Libro { Id = 1, Titulo = "A" },
                new Libro { Id = 2, Titulo = "B" });
            context.SaveChanges();

            var repo = new Repository<Libro>(context);
            var consulta = repo.ObtenerPorConsulta();

            var filtrados = consulta.Where(l => l.Titulo == "B").ToList();

            Assert.Single(filtrados);
            Assert.Equal("B", filtrados.First().Titulo);
        }

        [Fact]
        public async Task ObtenerPorId_DeberiaRetornarEntidadExistente()
        {
            using var context = CrearContexto();
            context.Libros.Add(new Libro { Id = 1, Titulo = "A" });
            await context.SaveChangesAsync();

            var repo = new Repository<Libro>(context);
            var entidad = await repo.ObtenerPorId(1);

            Assert.NotNull(entidad);
            Assert.Equal("A", entidad!.Titulo);
        }

        [Fact]
        public async Task ObtenerPorId_DeberiaRetornarNullSiNoExiste()
        {
            using var context = CrearContexto();
            var repo = new Repository<Libro>(context);

            var entidad = await repo.ObtenerPorId(99);

            Assert.Null(entidad);
        }

        [Fact]
        public async Task Agregar_DeberiaInsertarEntidad()
        {
            using var context = CrearContexto();
            var repo = new Repository<Libro>(context);

            var libro = new Libro { Id = 10, Titulo = "Nuevo" };
            await repo.Agregar(libro);

            var existe = await context.Libros.AnyAsync(l => l.Titulo == "Nuevo");
            Assert.True(existe);
        }

        [Fact]
        public async Task Actualizar_DeberiaActualizarEntidadExistente()
        {
            using var context = CrearContexto();
            context.Libros.Add(new Libro { Id = 1, Titulo = "Viejo" });
            await context.SaveChangesAsync();

            var repo = new Repository<Libro>(context);
            var actualizado = new Libro { Id = 1, Titulo = "Nuevo" };

            await repo.Actualizar(actualizado);

            var libroDb = await context.Libros.FindAsync(1);
            Assert.Equal("Nuevo", libroDb!.Titulo);
        }

        [Fact]
        public async Task Actualizar_DeberiaLanzarExcepcionSiNoExiste()
        {
            using var context = CrearContexto();
            var repo = new Repository<Libro>(context);

            var libro = new Libro { Id = 1, Titulo = "No existe" };

            var ex = await Assert.ThrowsAsync<Exception>(() => repo.Actualizar(libro));
            Assert.Equal("Entity not found", ex.Message);
        }

        [Fact]
        public async Task Eliminar_DeberiaEliminarEntidad()
        {
            using var context = CrearContexto();
            var libro = new Libro { Id = 1, Titulo = "Eliminar" };
            context.Libros.Add(libro);
            await context.SaveChangesAsync();

            var repo = new Repository<Libro>(context);
            await repo.Eliminar(libro);

            var existe = await context.Libros.FindAsync(1);
            Assert.Null(existe);
        }
    }
}
