using GestionBiblioteca.Entities;
using Xunit;

namespace GestionBiblioteca.Tests.Entities
{
    public class LibroTests
    {
        [Fact]
        public void Libro_Propiedades_AsignacionCorrecta()
        {
            var libro = new Libro
            {
                Id = 1,
                IdEditorial = 3,
                Titulo = "Cien años de soledad",
                Isbn = "9781234567890",
                Idioma = "Español"
            };

            Assert.Equal("Cien años de soledad", libro.Titulo);
            Assert.Equal("9781234567890", libro.Isbn);
            Assert.Equal("Español", libro.Idioma);
        }

        [Fact]
        public void Libro_Colecciones_InicializanVacias()
        {
            var libro = new Libro();
            Assert.NotNull(libro.Ejemplares);
            Assert.NotNull(libro.IdAutores);
            Assert.NotNull(libro.IdCategoria);
            Assert.Empty(libro.Ejemplares);
            Assert.Empty(libro.IdAutores);
            Assert.Empty(libro.IdCategoria);
        }
    }
}
