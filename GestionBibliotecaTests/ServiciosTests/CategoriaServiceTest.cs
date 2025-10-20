using GestionBiblioteca.Entities;
using GestionBiblioteca.Repository;
using GestionBiblioteca.Services.Categoria;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using Xunit;
using GestionBibliotecaTests.Helpers;

namespace GestionBibliotecaTests
{
    public class CategoriaServiceTest
    {
        private readonly Mock<IRepositoryFactory> _mockRepoFactory;
        private readonly Mock<IRepository<Categoria>> _mockCategoriaRepo;
        private readonly Mock<IHttpContextAccessor> _mockHttpContext;
        private readonly CategoriaService _categoriaService;

        public CategoriaServiceTest()
        {
            _mockRepoFactory = new Mock<IRepositoryFactory>();
            _mockCategoriaRepo = new Mock<IRepository<Categoria>>();
            _mockHttpContext = new Mock<IHttpContextAccessor>();

            _mockRepoFactory.Setup(f => f.ObtenerRepository<Categoria>())
                .Returns(_mockCategoriaRepo.Object);

            _categoriaService = new CategoriaService(_mockRepoFactory.Object, _mockHttpContext.Object);
        }

        // ObtenerIdSesion
        [Fact]
        public void ObtenerIdSesion_UsuarioAutenticadoConClaimValida()
        {
            var claims = new List<Claim> { new Claim("CategoriaId", "5") };
            var identity = new ClaimsIdentity(claims, "mock");
            var principal = new ClaimsPrincipal(identity);
            _mockHttpContext.Setup(h => h.HttpContext).Returns(new DefaultHttpContext { User = principal });

            var result = _categoriaService.GetType()
                .GetMethod("ObtenerIdSesion", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .Invoke(_categoriaService, null);

            Assert.Equal(5, result);
        }

        [Fact]
        public void ObtenerIdSesion_UsuarioAutenticadoSinClaim()
        {
            var identity = new ClaimsIdentity(new List<Claim>(), "mock");
            var principal = new ClaimsPrincipal(identity);
            _mockHttpContext.Setup(h => h.HttpContext).Returns(new DefaultHttpContext { User = principal });

            var result = _categoriaService.GetType()
                .GetMethod("ObtenerIdSesion", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .Invoke(_categoriaService, null);

            Assert.Equal(1, result);
        }

        [Fact]
        public void ObtenerIdSesion_UsuarioNoAutenticado()
        {
            _mockHttpContext.Setup(h => h.HttpContext).Returns(new DefaultHttpContext());

            var result = _categoriaService.GetType()
                .GetMethod("ObtenerIdSesion", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .Invoke(_categoriaService, null);

            Assert.Equal(1, result);
        }

        [Fact]
        public void ObtenerIdSesion_ClaimNoConvertibleAInt()
        {
            var claims = new List<Claim> { new Claim("CategoriaId", "abc") };
            var identity = new ClaimsIdentity(claims, "mock");
            var principal = new ClaimsPrincipal(identity);
            _mockHttpContext.Setup(h => h.HttpContext).Returns(new DefaultHttpContext { User = principal });

            var result = _categoriaService.GetType()
                .GetMethod("ObtenerIdSesion", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .Invoke(_categoriaService, null);

            Assert.Equal(1, result);
        }

        // ObtenerTodos
        [Fact]
        public async Task ObtenerTodos_RetornaTodos()
        {
            var categorias = new List<Categoria>
            {
                new Categoria { Id = 1, Nombre = "Categoría 1" },
                new Categoria { Id = 2, Nombre = "Categoría 2" }
            }.BuildMockAsyncQueryable();

            _mockCategoriaRepo.Setup(r => r.ObtenerPorConsulta()).Returns(categorias);

            var result = await _categoriaService.ObtenerTodos();

            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].Id);
            Assert.Equal(2, result[1].Id);
        }

        // ObtenerPorId
        [Fact]
        public async Task ObtenerPorId_CategoriaExiste()
        {
            var categoria = new Categoria { Id = 10, Nombre = "Categoría de Prueba" };
            _mockCategoriaRepo.Setup(r => r.ObtenerPorId(10)).ReturnsAsync(categoria);

            var result = await _categoriaService.ObtenerPorId(10);

            Assert.NotNull(result);
            Assert.Equal(10, result!.Id);
            Assert.Equal("Categoría de Prueba", result!.Nombre);
        }

        [Fact]
        public async Task ObtenerPorId_CategoriaNoExiste()
        {
            _mockCategoriaRepo.Setup(r => r.ObtenerPorId(20)).ReturnsAsync((Categoria?)null);

            var result = await _categoriaService.ObtenerPorId(20);

            Assert.Null(result);
        }

        // Crear
        [Fact]
        public async Task Crear_NuevaCategoria()
        {
            var categoria = new Categoria { Id = 1, Nombre = "Nueva Categoría" };

            var result = await _categoriaService.Crear(categoria);

            Assert.Equal("Nueva Categoría", result.Nombre);
            _mockCategoriaRepo.Verify(r => r.Agregar(It.IsAny<Categoria>()), Times.Once);
        }

        // Actualizar
        [Fact]
        public async Task Actualizar_CategoriaExiste_SeActualiza()
        {
            var existente = new Categoria { Id = 1, Nombre = "Nombre Original" };
            _mockCategoriaRepo.Setup(r => r.ObtenerPorId(1)).ReturnsAsync(existente);

            var modificado = new Categoria { Id = 1, Nombre = "Nombre Nuevo" };

            var result = await _categoriaService.Actualizar(modificado);

            Assert.Equal("Nombre Nuevo", result!.Nombre);
            _mockCategoriaRepo.Verify(r => r.Actualizar(It.IsAny<Categoria>()), Times.Once);
        }

        [Fact]
        public async Task Actualizar_CategoriaNoExiste_LanzaExcepcion()
        {
            _mockCategoriaRepo.Setup(r => r.ObtenerPorId(5)).ReturnsAsync((Categoria?)null);

            var categoria = new Categoria { Id = 5, Nombre = "No Existe" };

            await Assert.ThrowsAsync<Exception>(() => _categoriaService.Actualizar(categoria));
        }

        // Eliminar
        [Fact]
        public async Task Eliminar_CategoriaExiste()
        {
            var categoria = new Categoria { Id = 3 };
            _mockCategoriaRepo.Setup(r => r.ObtenerPorId(3)).ReturnsAsync(categoria);

            var result = await _categoriaService.Eliminar(3);

            Assert.True(result);
            _mockCategoriaRepo.Verify(r => r.Actualizar(It.IsAny<Categoria>()), Times.Once);
        }

        [Fact]
        public async Task Eliminar_CategoriaNoExiste()
        {
            _mockCategoriaRepo.Setup(r => r.ObtenerPorId(99)).ReturnsAsync((Categoria?)null);

            var result = await _categoriaService.Eliminar(99);

            Assert.False(result);
            _mockCategoriaRepo.Verify(r => r.Actualizar(It.IsAny<Categoria>()), Times.Never);
        }
    }
}