using GestionBiblioteca.Context;
using GestionBiblioteca.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Xunit;
using System;
using System.Threading.Tasks;

namespace GestionBiblioteca.Tests
{
    public class RepositoryFactoryTests
    {
        private MyDbContext CrearContexto()
        {
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            return new MyDbContext(options);
        }

        [Fact]
        public void ObtenerRepository_DeberiaRetornarInstanciaDeRepository()
        {
            using var context = CrearContexto();
            var factory = new RepositoryFactory(context, null!);

            var repo = factory.ObtenerRepository<object>();

            Assert.NotNull(repo);
            Assert.IsType<Repository<object>>(repo);
        }

        [Fact]
        public async Task BeginTransaction_DeberiaNoLanzarExcepcionYRetornarNullEnInMemory()
        {
            using var context = CrearContexto();
            var factory = new RepositoryFactory(context, null!);

            var resultado = await factory.BeginTransaction();

            // En InMemory, debe ser null (porque no soporta transacciones)
            Assert.Null(resultado);
        }
    }
}
