using GestionBiblioteca.Entities;
using GestionBiblioteca.Repository;
using GestionBiblioteca.Services.Prestamo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Xunit;
using GestionBibliotecaTests.Helpers;

namespace GestionBibliotecaTests
{
    public class PrestamoServiceTest
    {
        private readonly Mock<IRepositoryFactory> _mockRepoFactory;
        private readonly Mock<IRepository<Prestamo>> _mockPrestamoRepo;
        private readonly Mock<IRepository<PrestamoEjemplar>> _mockPrestamoEjemplarRepo;
        private readonly Mock<IRepository<Ejemplar>> _mockEjemplarRepo;
        private readonly Mock<IDbContextTransaction> _mockTransaction;
        private readonly PrestamoService _prestamoService;

        public PrestamoServiceTest()
        {
            _mockRepoFactory = new Mock<IRepositoryFactory>();
            _mockPrestamoRepo = new Mock<IRepository<Prestamo>>();
            _mockPrestamoEjemplarRepo = new Mock<IRepository<PrestamoEjemplar>>();
            _mockEjemplarRepo = new Mock<IRepository<Ejemplar>>();
            _mockTransaction = new Mock<IDbContextTransaction>();

            _mockRepoFactory.Setup(f => f.ObtenerRepository<Prestamo>())
                .Returns(_mockPrestamoRepo.Object);
            
            _mockRepoFactory.Setup(f => f.ObtenerRepository<PrestamoEjemplar>())
                .Returns(_mockPrestamoEjemplarRepo.Object);
            
            _mockRepoFactory.Setup(f => f.ObtenerRepository<Ejemplar>())
                .Returns(_mockEjemplarRepo.Object);

            _mockRepoFactory.Setup(f => f.BeginTransaction())
                .ReturnsAsync(_mockTransaction.Object);

            _prestamoService = new PrestamoService(_mockRepoFactory.Object);
        }

        [Fact]
        public async Task ObtenerTodos_RetornaPrestamosFiltrados()
        {
            // Arrange
            var prestamos = new List<Prestamo>
            {
                new Prestamo { Id = 1, Activo = 1, IdUsuarioNavigation = new Usuario { Id = 1 } },
                new Prestamo { Id = 2, Activo = 0, IdUsuarioNavigation = new Usuario { Id = 2 } },
                new Prestamo { Id = 3, Activo = 1, IdUsuarioNavigation = new Usuario { Id = 3 } }
            };
            
            var mockDbSet = prestamos.AsQueryable().BuildMockAsyncQueryable();

            _mockPrestamoRepo.Setup(r => r.ObtenerPorConsulta()).Returns(mockDbSet);

            // Act
            var result = await _prestamoService.ObtenerTodos();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, p => p.Id == 1);
            Assert.Contains(result, p => p.Id == 3);
            Assert.DoesNotContain(result, p => p.Id == 2); 
        }

        [Fact]
        public async Task ObtenerPorId_PrestamoExiste()
        {

            var prestamo = new Prestamo { 
                Id = 10, 
                IdUsuario = 5, 
                IdUsuarioNavigation = new Usuario { Id = 5 },
                PrestamoEjemplares = new List<PrestamoEjemplar> 
                { 
                    new PrestamoEjemplar 
                    { 
                        IdEjemplar = 7, 
                        IdEjemplarNavigation = new Ejemplar { 
                            Id = 7, 
                            IdLibro = 3,
                            IdLibroNavigation = new Libro { Id = 3, Titulo = "Test Book" }
                        } 
                    } 
                }
            };

            var mockService = new Mock<IPrestamoService>();
            mockService.Setup(s => s.ObtenerPorId(10)).ReturnsAsync(prestamo);

            var result = await mockService.Object.ObtenerPorId(10);

            Assert.NotNull(result);
            Assert.Equal(10, result!.Id);
            Assert.Equal(5, result.IdUsuarioNavigation?.Id);
            Assert.Single(result.PrestamoEjemplares);
            Assert.Equal("Test Book", result.PrestamoEjemplares.First().IdEjemplarNavigation.IdLibroNavigation?.Titulo);

            mockService.Verify(s => s.ObtenerPorId(10), Times.Once());
        }

        [Fact]
        public async Task ObtenerPorId_PrestamoNoExiste()
        {
            var mockService = new Mock<IPrestamoService>();
            mockService.Setup(s => s.ObtenerPorId(20)).ReturnsAsync((Prestamo)null);
            
            var result = await mockService.Object.ObtenerPorId(20);
            Assert.Null(result);
            mockService.Verify(s => s.ObtenerPorId(20), Times.Once());
        }

        [Fact]
        public async Task Crear_GuardaPrestamoYActualizaEjemplares()
        {
            // Arrange
            int idUsuario = 5;
            var ejemplares = new List<PrestamoEjemplar>
            {
                new PrestamoEjemplar { IdEjemplar = 1, FechaLimite = DateTime.Today.AddDays(7) },
                new PrestamoEjemplar { IdEjemplar = 2, FechaLimite = DateTime.Today.AddDays(14) }
            };
            
            var ejemplaresEntities = new List<Ejemplar>
            {
                new Ejemplar { Id = 1, Disponible = true },
                new Ejemplar { Id = 2, Disponible = true }
            };

            _mockEjemplarRepo.Setup(r => r.ObtenerPorId(1)).ReturnsAsync(ejemplaresEntities[0]);
            _mockEjemplarRepo.Setup(r => r.ObtenerPorId(2)).ReturnsAsync(ejemplaresEntities[1]);

            // Act
            await _prestamoService.Crear(idUsuario, ejemplares);

            // Assert
            _mockPrestamoRepo.Verify(r => r.Agregar(It.Is<Prestamo>(p => 
                p.IdUsuario == idUsuario && 
                p.FechaPrestamo.HasValue && 
                p.FechaPrestamo.Value.Date == DateTime.Now.Date && 
                p.Activo == 1 && 
                p.Cancelado == false)), Times.Once);
                
            _mockPrestamoEjemplarRepo.Verify(r => r.Agregar(It.Is<PrestamoEjemplar>(pe => 
                pe.IdEjemplar == 1 && 
                pe.FechaLimite == ejemplares[0].FechaLimite &&
                pe.Activo == 1)), Times.Once);
                
            _mockPrestamoEjemplarRepo.Verify(r => r.Agregar(It.Is<PrestamoEjemplar>(pe => 
                pe.IdEjemplar == 2 && 
                pe.FechaLimite == ejemplares[1].FechaLimite &&
                pe.Activo == 1)), Times.Once);
            
            _mockEjemplarRepo.Verify(r => r.Actualizar(It.Is<Ejemplar>(e => e.Id == 1 && e.Disponible == false)), Times.Once);
            _mockEjemplarRepo.Verify(r => r.Actualizar(It.Is<Ejemplar>(e => e.Id == 2 && e.Disponible == false)), Times.Once);
            
            _mockTransaction.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Crear_ErrorEnTransaccion_HaceRollback()
        {
            // Arrange
            int idUsuario = 5;
            var ejemplares = new List<PrestamoEjemplar>
            {
                new PrestamoEjemplar { IdEjemplar = 1, FechaLimite = DateTime.Today.AddDays(7) },
                new PrestamoEjemplar { IdEjemplar = 2, FechaLimite = DateTime.Today.AddDays(14) }
            };
            
            _mockPrestamoRepo.Setup(r => r.Agregar(It.IsAny<Prestamo>())).ThrowsAsync(new Exception("Error simulado"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await _prestamoService.Crear(idUsuario, ejemplares));
            
            _mockTransaction.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockTransaction.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}