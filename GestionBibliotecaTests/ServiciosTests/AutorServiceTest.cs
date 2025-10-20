using GestionBiblioteca.Entities;
using GestionBiblioteca.Repository;
using GestionBiblioteca.Services.Autor;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using Xunit;
using GestionBibliotecaTests.Helpers;

namespace GestionBibliotecaTests
{
    public class AutorServiceTest
    {
        private readonly Mock<IRepositoryFactory> _mockRepoFactory;
        private readonly Mock<IRepository<Autor>> _mockAutorRepo;
        private readonly Mock<IHttpContextAccessor> _mockHttpContext;
        private readonly AutorService _autorService;

        public AutorServiceTest()
        {
            _mockRepoFactory = new Mock<IRepositoryFactory>();
            _mockAutorRepo = new Mock<IRepository<Autor>>();
            _mockHttpContext = new Mock<IHttpContextAccessor>();

            _mockRepoFactory.Setup(f => f.ObtenerRepository<Autor>())
                .Returns(_mockAutorRepo.Object);

            _autorService = new AutorService(_mockRepoFactory.Object, _mockHttpContext.Object);
        }

        // ObtenerIdSesion
        [Fact]
        public void ObtenerIdSesion_UsuarioAutenticadoConClaimValida()
        {
            var claims = new List<Claim> { new Claim("AutorId", "5") };
            var identity = new ClaimsIdentity(claims, "mock");
            var principal = new ClaimsPrincipal(identity);
            _mockHttpContext.Setup(h => h.HttpContext).Returns(new DefaultHttpContext { User = principal });

            var result = _autorService.GetType()
                .GetMethod("ObtenerIdSesion", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .Invoke(_autorService, null);

            Assert.Equal(5, result);
        }

        [Fact]
        public void ObtenerIdSesion_UsuarioAutenticadoSinClaim()
        {
            var identity = new ClaimsIdentity(new List<Claim>(), "mock");
            var principal = new ClaimsPrincipal(identity);
            _mockHttpContext.Setup(h => h.HttpContext).Returns(new DefaultHttpContext { User = principal });

            var result = _autorService.GetType()
                .GetMethod("ObtenerIdSesion", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .Invoke(_autorService, null);

            Assert.Equal(1, result);
        }

        [Fact]
        public void ObtenerIdSesion_UsuarioNoAutenticado()
        {
            _mockHttpContext.Setup(h => h.HttpContext).Returns(new DefaultHttpContext());

            var result = _autorService.GetType()
                .GetMethod("ObtenerIdSesion", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .Invoke(_autorService, null);

            Assert.Equal(1, result);
        }

        [Fact]
        public void ObtenerIdSesion_ClaimNoConvertibleAInt()
        {
            var claims = new List<Claim> { new Claim("AutorId", "abc") };
            var identity = new ClaimsIdentity(claims, "mock");
            var principal = new ClaimsPrincipal(identity);
            _mockHttpContext.Setup(h => h.HttpContext).Returns(new DefaultHttpContext { User = principal });

            var result = _autorService.GetType()
                .GetMethod("ObtenerIdSesion", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .Invoke(_autorService, null);

            Assert.Equal(1, result);
        }

        // ObtenerTodos
        [Fact]
        public async Task ObtenerTodos_RetornaTodos()
        {
            var autores = new List<Autor>
            {
                new Autor { Id = 1, Nombre = "Autor 1" },
                new Autor { Id = 2, Nombre = "Autor 2" }
            }.BuildMockAsyncQueryable();

            _mockAutorRepo.Setup(r => r.ObtenerPorConsulta()).Returns(autores);

            var result = await _autorService.ObtenerTodos();

            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].Id);
            Assert.Equal(2, result[1].Id);
        }

        // ObtenerPorId
        [Fact]
        public async Task ObtenerPorId_AutorExiste()
        {
            var autor = new Autor { Id = 10, Nombre = "Autor de Prueba" };
            _mockAutorRepo.Setup(r => r.ObtenerPorId(10)).ReturnsAsync(autor);

            var result = await _autorService.ObtenerPorId(10);

            Assert.NotNull(result);
            Assert.Equal(10, result!.Id);
            Assert.Equal("Autor de Prueba", result!.Nombre);
        }

        [Fact]
        public async Task ObtenerPorId_AutorNoExiste()
        {
            _mockAutorRepo.Setup(r => r.ObtenerPorId(20)).ReturnsAsync((Autor?)null);

            var result = await _autorService.ObtenerPorId(20);

            Assert.Null(result);
        }

        // Crear
        [Fact]
        public async Task Crear_NuevoAutor()
        {
            var autor = new Autor { Id = 1, Nombre = "Nuevo Autor" };

            var result = await _autorService.Crear(autor);

            Assert.Equal("Nuevo Autor", result.Nombre);
            _mockAutorRepo.Verify(r => r.Agregar(It.IsAny<Autor>()), Times.Once);
        }

        // Actualizar
        [Fact]
        public async Task Actualizar_AutorExiste_SeActualiza()
        {
            var existente = new Autor { Id = 1, Nombre = "Nombre Original" };
            _mockAutorRepo.Setup(r => r.ObtenerPorId(1)).ReturnsAsync(existente);

            var modificado = new Autor { Id = 1, Nombre = "Nombre Nuevo" };

            var result = await _autorService.Actualizar(modificado);

            Assert.Equal("Nombre Nuevo", result!.Nombre);
            _mockAutorRepo.Verify(r => r.Actualizar(It.IsAny<Autor>()), Times.Once);
        }

        [Fact]
        public async Task Actualizar_AutorNoExiste_LanzaExcepcion()
        {
            _mockAutorRepo.Setup(r => r.ObtenerPorId(5)).ReturnsAsync((Autor?)null);

            var autor = new Autor { Id = 5, Nombre = "No Existe" };

            await Assert.ThrowsAsync<Exception>(() => _autorService.Actualizar(autor));
        }

        // Eliminar
        [Fact]
        public async Task Eliminar_AutorExiste()
        {
            var autor = new Autor { Id = 3 };
            _mockAutorRepo.Setup(r => r.ObtenerPorId(3)).ReturnsAsync(autor);

            var result = await _autorService.Eliminar(3);

            Assert.True(result);
            _mockAutorRepo.Verify(r => r.Actualizar(It.IsAny<Autor>()), Times.Once);
        }

        [Fact]
        public async Task Eliminar_AutorNoExiste()
        {
            _mockAutorRepo.Setup(r => r.ObtenerPorId(99)).ReturnsAsync((Autor?)null);

            var result = await _autorService.Eliminar(99);

            Assert.False(result);
            _mockAutorRepo.Verify(r => r.Actualizar(It.IsAny<Autor>()), Times.Never);
        }
    }
}