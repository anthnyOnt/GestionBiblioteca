using GestionBiblioteca.Entities;
using Xunit;

namespace GestionBiblioteca.Tests.Entities
{
    public class EjemplarTests
    {
        [Fact]
        public void Ejemplar_Propiedades_AsignacionCorrecta()
        {
            var ejemplar = new Ejemplar
            {
                Id = 10,
                IdLibro = 2,
                Disponible = true,
                Activo = 1
            };

            Assert.Equal(10, ejemplar.Id);
            Assert.Equal(2, ejemplar.IdLibro);
            Assert.True(ejemplar.Disponible);
            Assert.Equal(1, ejemplar.Activo);
        }

        [Fact]
        public void Ejemplar_Colecciones_InicializanVacias()
        {
            var ejemplar = new Ejemplar();

            Assert.NotNull(ejemplar.Estados);
            Assert.NotNull(ejemplar.PrestamoEjemplares);
            Assert.Empty(ejemplar.Estados);
            Assert.Empty(ejemplar.PrestamoEjemplares);
        }
    }
}
