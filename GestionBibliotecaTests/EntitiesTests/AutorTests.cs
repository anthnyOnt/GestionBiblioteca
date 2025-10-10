using GestionBiblioteca.Entities;
using Xunit;

namespace GestionBiblioteca.Tests.Entities
{
    public class AutorTests
    {
        [Fact]
        public void Autor_Propiedades_AsignacionCorrecta()
        {
            // Arrange
            var autor = new Autor
            {
                Id = 1,
                Nombre = "Gabriel",
                Apellido = "García Márquez"
            };

            // Assert
            Assert.Equal(1, autor.Id);
            Assert.Equal("Gabriel", autor.Nombre);
            Assert.Equal("García Márquez", autor.Apellido);
        }

        [Fact]
        public void Autor_ListaLibros_InicializaVacia()
        {
            var autor = new Autor();
            Assert.NotNull(autor.IdLibros);
            Assert.Empty(autor.IdLibros);
        }
    }
}

