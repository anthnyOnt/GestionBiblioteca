using GestionBiblioteca.Context;
using GestionBiblioteca.Entities;
using GestionBiblioteca.Repository;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;
using System.Linq;

public class RepositoryTests
{
    private MyDbContext CrearContexto()
    {
        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new MyDbContext(options);
    }
}