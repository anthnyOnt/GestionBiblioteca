using GestionBiblioteca.Entities;
using Xunit;

namespace GestionBiblioteca.Tests.Entities
{
    public class EditorialTests
    {
        [Fact]
        public void Editorial_Propiedades_AsignacionCorrecta()
        {
            var editorial = new Editorial
            {
                Id = 1,
                Nombre = "Alfaguara",
                Telefono = "123456789",
                Correo = "contacto@alfaguara.com",
                SitioWeb = "https://alfaguara.com"
            };

            Assert.Equal("Alfaguara", editorial.Nombre);
            Assert.Equal("123456789", editorial.Telefono);
            Assert.Equal("contacto@alfaguara.com", editorial.Correo);
            Assert.Equal("https://alfaguara.com", editorial.SitioWeb);
        }

        [Fact]
        public void Editorial_Libros_InicializaVacia()
        {
            var editorial = new Editorial();
            Assert.NotNull(editorial.Libros);
            Assert.Empty(editorial.Libros);
        }
    }
}
