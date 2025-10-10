using GestionBiblioteca.Entities;
using Xunit;

namespace GestionBiblioteca.Tests.Entities
{
    public class PrestamoTests
    {
        [Fact]
        public void Prestamo_Propiedades_AsignacionCorrecta()
        {
            var prestamo = new Prestamo
            {
                Id = 100,
                IdUsuario = 5,
                Cancelado = false,
                Activo = 1
            };

            Assert.Equal(100, prestamo.Id);
            Assert.Equal(5, prestamo.IdUsuario);
            Assert.False(prestamo.Cancelado);
            Assert.Equal(1, prestamo.Activo);
        }

        [Fact]
        public void Prestamo_Coleccion_InicializaVacia()
        {
            var prestamo = new Prestamo();
            Assert.NotNull(prestamo.PrestamoEjemplares);
            Assert.Empty(prestamo.PrestamoEjemplares);
        }
    }
}
