using GestionBiblioteca.Entities;
using Xunit;

namespace GestionBiblioteca.Tests.Entities
{
    public class EstadoTests
    {
        [Fact]
        public void Estado_Propiedades_AsignacionCorrecta()
        {
            var estado = new Estado
            {
                Id = 5,
                IdEjemplar = 10,
                Observacion = "Buen estado",
                Activo = 1
            };

            Assert.Equal(5, estado.Id);
            Assert.Equal(10, estado.IdEjemplar);
            Assert.Equal("Buen estado", estado.Observacion);
            Assert.Equal(1, estado.Activo);
        }
    }
}
