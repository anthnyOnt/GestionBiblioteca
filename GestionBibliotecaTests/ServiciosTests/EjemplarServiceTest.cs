using GestionBiblioteca.Entities;
using GestionBiblioteca.Repository;
using GestionBiblioteca.Services.Ejemplar;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using GestionBibliotecaTests.Helpers;
using Microsoft.AspNetCore.Http;

namespace GestionBibliotecaTests
{
    public class EjemplarServiceTest
    {
        private readonly Mock<IRepositoryFactory> _mockRepoFactory;
        private readonly Mock<IRepository<Ejemplar>> _mockEjemplarRepo;
        private readonly EjemplarService _ejemplarService;
        private readonly Mock<IHttpContextAccessor> _mockHttpContext;

        public EjemplarServiceTest()
        {
            _mockRepoFactory = new Mock<IRepositoryFactory>();
            _mockEjemplarRepo = new Mock<IRepository<Ejemplar>>();
            _mockHttpContext = new Mock<IHttpContextAccessor>();

            // Configuramos que el factory siempre devuelva el repo mock
            _mockRepoFactory.Setup(f => f.ObtenerRepository<Ejemplar>())
                .Returns(_mockEjemplarRepo.Object);

            _ejemplarService = new EjemplarService(_mockRepoFactory.Object,  _mockHttpContext.Object);
        }

        // ObtenerDisponibles
        [Fact]
        public async Task ObtenerDisponibles_RetornaEjemplaresActivosYDisponibles()
        {
            // Arrange
            var ejemplares = new List<Ejemplar>
            {
                new Ejemplar { Id = 1, Activo = 1, Disponible = true, IdLibro = 10, IdLibroNavigation = new Libro { Id = 10, Titulo = "Libro 1" } },
                new Ejemplar { Id = 2, Activo = 1, Disponible = false, IdLibro = 10, IdLibroNavigation = new Libro { Id = 10, Titulo = "Libro 1" } },
                new Ejemplar { Id = 3, Activo = 0, Disponible = true, IdLibro = 20, IdLibroNavigation = new Libro { Id = 20, Titulo = "Libro 2" } },
                new Ejemplar { Id = 4, Activo = 1, Disponible = true, IdLibro = 20, IdLibroNavigation = new Libro { Id = 20, Titulo = "Libro 2" } }
            };

            var mockDbSet = ejemplares.AsQueryable().BuildMockAsyncQueryable();
            _mockEjemplarRepo.Setup(r => r.ObtenerPorConsulta()).Returns(mockDbSet);

            // Act
            var result = await _ejemplarService.ObtenerDisponibles();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, e => e.Id == 1);
            Assert.Contains(result, e => e.Id == 4);
            Assert.DoesNotContain(result, e => e.Id == 2); // No disponible
            Assert.DoesNotContain(result, e => e.Id == 3); // No activo
        }

        // ObtenerSeleccionados
        [Fact]
        public async Task ObtenerSeleccionados_RetornaEjemplaresSeleccionados()
        {
            // Arrange
            var ejemplares = new List<Ejemplar>
            {
                new Ejemplar { Id = 1, IdLibro = 10, IdLibroNavigation = new Libro { Id = 10, Titulo = "Libro 1" } },
                new Ejemplar { Id = 2, IdLibro = 10, IdLibroNavigation = new Libro { Id = 10, Titulo = "Libro 1" } },
                new Ejemplar { Id = 3, IdLibro = 20, IdLibroNavigation = new Libro { Id = 20, Titulo = "Libro 2" } },
                new Ejemplar { Id = 4, IdLibro = 20, IdLibroNavigation = new Libro { Id = 20, Titulo = "Libro 2" } }
            };

            var mockDbSet = ejemplares.AsQueryable().BuildMockAsyncQueryable();
            _mockEjemplarRepo.Setup(r => r.ObtenerPorConsulta()).Returns(mockDbSet);

            var seleccionados = new List<int> { 1, 3 };

            // Act
            var result = await _ejemplarService.ObtenerSeleccionados(seleccionados);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, e => e.Id == 1);
            Assert.Contains(result, e => e.Id == 3);
            Assert.DoesNotContain(result, e => e.Id == 2);
            Assert.DoesNotContain(result, e => e.Id == 4);
        }

        [Fact]
        public async Task ObtenerSeleccionados_ListaVacia_RetornaListaVacia()
        {
            // Arrange
            var ejemplares = new List<Ejemplar>
            {
                new Ejemplar { Id = 1 },
                new Ejemplar { Id = 2 }
            };

            var mockDbSet = ejemplares.AsQueryable().BuildMockAsyncQueryable();
            _mockEjemplarRepo.Setup(r => r.ObtenerPorConsulta()).Returns(mockDbSet);

            var seleccionados = new List<int>();

            // Act
            var result = await _ejemplarService.ObtenerSeleccionados(seleccionados);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task ObtenerSeleccionados_EjemplarNoExiste_NoLoIncluye()
        {
            // Arrange
            var ejemplares = new List<Ejemplar>
            {
                new Ejemplar { Id = 1 }
            };

            var mockDbSet = ejemplares.AsQueryable().BuildMockAsyncQueryable();
            _mockEjemplarRepo.Setup(r => r.ObtenerPorConsulta()).Returns(mockDbSet);

            var seleccionados = new List<int> { 1, 999 };  // 999 no existe

            // Act
            var result = await _ejemplarService.ObtenerSeleccionados(seleccionados);

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result[0].Id);
        }
    }
}