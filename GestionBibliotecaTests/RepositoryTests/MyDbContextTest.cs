using GestionBiblioteca.Context;
using GestionBiblioteca.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestionBiblioteca.Tests
{
    public class MyDbContextTest : MyDbContext
    {
        public MyDbContextTest(DbContextOptions<MyDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }
    }
}
