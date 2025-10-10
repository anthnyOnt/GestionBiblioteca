using GestionBiblioteca.Entities;
using Xunit;

namespace GestionBiblioteca.Tests.Entities
{
    public class PrestamoEjemplarTests
    {
        [Fact]
        public void PrestamoEjemplar_Propiedades_AsignacionCorrecta()
        {
            var prestamoEjemplar = new PrestamoEjemplar
            {
                IdPrestamo = 1,
                IdEjemplar = 2,
                Devuelto = false,
                Activo = 1
            };

            Assert.Equal(1, prestamoEjemplar.IdPrestamo);
            Assert.Equal(2, prestamoEjemplar.IdEjemplar);
            Assert.False(prestamoEjemplar.Devuelto);
            Assert.Equal(1, prestamoEjemplar.Activo);
        }
    }
}
