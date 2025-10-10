using GestionBiblioteca.Entities;
using Xunit;

namespace GestionBiblioteca.Tests.Entities
{
    public class CategoriaTests
    {
        [Fact]
        public void Categoria_Propiedades_AsignacionCorrecta()
        {
            var categoria = new Categoria
            {
                Id = 1,
                Nombre = "Novela",
                Activo = 1
            };

            Assert.Equal((sbyte)1, categoria.Id);
            Assert.Equal("Novela", categoria.Nombre);
            Assert.Equal(1, categoria.Activo);
        }

        [Fact]
        public void Categoria_ListaLibros_InicializaVacia()
        {
            var categoria = new Categoria();
            Assert.NotNull(categoria.IdLibros);
            Assert.Empty(categoria.IdLibros);
        }
    }
}
