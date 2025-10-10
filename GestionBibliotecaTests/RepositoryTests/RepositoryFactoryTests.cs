using GestionBiblioteca.Context;
using GestionBiblioteca.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Xunit;
using System;
using System.Threading.Tasks;

namespace GestionBiblioteca.Tests
{
    public class RepositoryFactoryTests
    {
        private MyDbContextTest CrearContexto()
        {
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new MyDbContextTest(options);
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

    }
}
