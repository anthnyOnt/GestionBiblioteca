using GestionBiblioteca.Entities;
using GestionBiblioteca.Repository;
using GestionBiblioteca.Services.Editorial;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using Xunit;
using GestionBibliotecaTests.Helpers;

namespace GestionBibliotecaTests
{
    public class EditorialServiceTest
    {
        private readonly Mock<IRepositoryFactory> _mockRepoFactory;
        private readonly Mock<IRepository<Editorial>> _mockEditorialRepo;
        private readonly Mock<IHttpContextAccessor> _mockHttpContext;
        private readonly EditorialService _editorialService;

        public EditorialServiceTest()
        {
            _mockRepoFactory = new Mock<IRepositoryFactory>();
            _mockEditorialRepo = new Mock<IRepository<Editorial>>();
            _mockHttpContext = new Mock<IHttpContextAccessor>();

            _mockRepoFactory.Setup(f => f.ObtenerRepository<Editorial>())
                .Returns(_mockEditorialRepo.Object);

            _editorialService = new EditorialService(_mockRepoFactory.Object, _mockHttpContext.Object);
        }

        // ObtenerIdSesion
        [Fact]
        public void ObtenerIdSesion_UsuarioAutenticadoConClaimValida()
        {
            var claims = new List<Claim> { new Claim("EditorialId", "5") };
            var identity = new ClaimsIdentity(claims, "mock");
            var principal = new ClaimsPrincipal(identity);
            _mockHttpContext.Setup(h => h.HttpContext).Returns(new DefaultHttpContext { User = principal });

            var result = _editorialService.GetType()
                .GetMethod("ObtenerIdSesion", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .Invoke(_editorialService, null);

            Assert.Equal(5, result);
        }

        [Fact]
        public void ObtenerIdSesion_UsuarioAutenticadoSinClaim()
        {
            var identity = new ClaimsIdentity(new List<Claim>(), "mock");
            var principal = new ClaimsPrincipal(identity);
            _mockHttpContext.Setup(h => h.HttpContext).Returns(new DefaultHttpContext { User = principal });

            var result = _editorialService.GetType()
                .GetMethod("ObtenerIdSesion", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .Invoke(_editorialService, null);

            Assert.Equal(1, result);
        }

        [Fact]
        public void ObtenerIdSesion_UsuarioNoAutenticado()
        {
            _mockHttpContext.Setup(h => h.HttpContext).Returns(new DefaultHttpContext());

            var result = _editorialService.GetType()
                .GetMethod("ObtenerIdSesion", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .Invoke(_editorialService, null);

            Assert.Equal(1, result);
        }

        [Fact]
        public void ObtenerIdSesion_ClaimNoConvertibleAInt()
        {
            var claims = new List<Claim> { new Claim("EditorialId", "abc") };
            var identity = new ClaimsIdentity(claims, "mock");
            var principal = new ClaimsPrincipal(identity);
            _mockHttpContext.Setup(h => h.HttpContext).Returns(new DefaultHttpContext { User = principal });

            var result = _editorialService.GetType()
                .GetMethod("ObtenerIdSesion", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .Invoke(_editorialService, null);

            Assert.Equal(1, result);
        }

        // ObtenerTodos
        [Fact]
        public async Task ObtenerTodos_RetornaTodos()
        {
            // Arrange
            var editoriales = new List<Editorial>
            {
                new Editorial { Id = 1, Nombre = "Editorial 1" },
                new Editorial { Id = 2, Nombre = "Editorial 2" }
            };
            
            var mockDbSet = editoriales.AsQueryable().BuildMockAsyncQueryable();
            _mockEditorialRepo.Setup(r => r.ObtenerPorConsulta()).Returns(mockDbSet);

            // Act
            var result = await _editorialService.ObtenerTodos();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, e => e.Id == 1 && e.Nombre == "Editorial 1");
            Assert.Contains(result, e => e.Id == 2 && e.Nombre == "Editorial 2");
        }

        // ObtenerPorId
        [Fact]
        public async Task ObtenerPorId_EditorialExiste()
        {
            var editorial = new Editorial { Id = 10, Nombre = "Editorial de Prueba" };
            _mockEditorialRepo.Setup(r => r.ObtenerPorId(10)).ReturnsAsync(editorial);

            var result = await _editorialService.ObtenerPorId(10);

            Assert.NotNull(result);
            Assert.Equal(10, result!.Id);
            Assert.Equal("Editorial de Prueba", result!.Nombre);
        }

        [Fact]
        public async Task ObtenerPorId_EditorialNoExiste()
        {
            _mockEditorialRepo.Setup(r => r.ObtenerPorId(20)).ReturnsAsync((Editorial?)null);

            var result = await _editorialService.ObtenerPorId(20);

            Assert.Null(result);
        }

        // Crear
        [Fact]
        public async Task Crear_NuevaEditorial()
        {
            var editorial = new Editorial { Id = 1, Nombre = "Nueva Editorial" };

            var result = await _editorialService.Crear(editorial);

            Assert.Equal("Nueva Editorial", result.Nombre);
            _mockEditorialRepo.Verify(r => r.Agregar(It.IsAny<Editorial>()), Times.Once);
        }

        // Actualizar
        [Fact]
        public async Task Actualizar_EditorialExiste_SeActualiza()
        {
            var existente = new Editorial { Id = 1, Nombre = "Nombre Original" };
            _mockEditorialRepo.Setup(r => r.ObtenerPorId(1)).ReturnsAsync(existente);

            var modificado = new Editorial { Id = 1, Nombre = "Nombre Nuevo" };

            var result = await _editorialService.Actualizar(modificado);

            Assert.Equal("Nombre Nuevo", result!.Nombre);
            _mockEditorialRepo.Verify(r => r.Actualizar(It.IsAny<Editorial>()), Times.Once);
        }

        [Fact]
        public async Task Actualizar_EditorialNoExiste_LanzaExcepcion()
        {
            _mockEditorialRepo.Setup(r => r.ObtenerPorId(5)).ReturnsAsync((Editorial?)null);

            var editorial = new Editorial { Id = 5, Nombre = "No Existe" };

            await Assert.ThrowsAsync<Exception>(() => _editorialService.Actualizar(editorial));
        }

        // Eliminar
        [Fact]
        public async Task Eliminar_EditorialExiste()
        {
            var editorial = new Editorial { Id = 3 };
            _mockEditorialRepo.Setup(r => r.ObtenerPorId(3)).ReturnsAsync(editorial);

            var result = await _editorialService.Eliminar(3);

            Assert.True(result);
            _mockEditorialRepo.Verify(r => r.Actualizar(It.IsAny<Editorial>()), Times.Once);
        }

        [Fact]
        public async Task Eliminar_EditorialNoExiste()
        {
            _mockEditorialRepo.Setup(r => r.ObtenerPorId(99)).ReturnsAsync((Editorial?)null);

            var result = await _editorialService.Eliminar(99);

            Assert.False(result);
            _mockEditorialRepo.Verify(r => r.Actualizar(It.IsAny<Editorial>()), Times.Never);
        }
    }
}