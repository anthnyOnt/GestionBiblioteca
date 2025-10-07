using GestionBiblioteca.Entities;
using GestionBiblioteca.Repository;
using GestionBiblioteca.Services.Usuario;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Security.Claims;
using Xunit;
using GestionBibliotecaTests.Helpers;

namespace GestionBibliotecaTests
{
    public class UsuarioServiceTest
    {
        private readonly Mock<IRepositoryFactory> _mockRepoFactory;
        private readonly Mock<IRepository<Usuario>> _mockUsuarioRepo;
        private readonly Mock<IHttpContextAccessor> _mockHttpContext;
        private readonly UsuarioService _usuarioService;

        public UsuarioServiceTest()
        {
            _mockRepoFactory = new Mock<IRepositoryFactory>();
            _mockUsuarioRepo = new Mock<IRepository<Usuario>>();
            _mockHttpContext = new Mock<IHttpContextAccessor>();

            // Configuramos que el factory siempre devuelva el repo mock
            _mockRepoFactory.Setup(f => f.ObtenerRepository<Usuario>())
                .Returns(_mockUsuarioRepo.Object);

            _usuarioService = new UsuarioService(_mockRepoFactory.Object, _mockHttpContext.Object);
        }

        // ObtenerIdSesion
        [Fact]
        public void ObtenerIdSesion_UsuarioAutenticadoConClaimValida()
        {
            var claims = new List<Claim> { new Claim("UsuarioId", "5") };
            var identity = new ClaimsIdentity(claims, "mock");
            var principal = new ClaimsPrincipal(identity);
            _mockHttpContext.Setup(h => h.HttpContext).Returns(new DefaultHttpContext { User = principal });

            var result = _usuarioService.GetType()
                .GetMethod("ObtenerIdSesion", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .Invoke(_usuarioService, null);

            Assert.Equal(5, result);
        }

        [Fact]
        public void ObtenerIdSesion_UsuarioAutenticadoSinClaim()
        {
            var identity = new ClaimsIdentity(new List<Claim>(), "mock");
            var principal = new ClaimsPrincipal(identity);
            _mockHttpContext.Setup(h => h.HttpContext).Returns(new DefaultHttpContext { User = principal });

            var result = _usuarioService.GetType()
                .GetMethod("ObtenerIdSesion", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .Invoke(_usuarioService, null);

            Assert.Equal(1, result); // valor por defecto
        }

        [Fact]
        public void ObtenerIdSesion_UsuarioNoAutenticado()
        {
            _mockHttpContext.Setup(h => h.HttpContext).Returns(new DefaultHttpContext());

            var result = _usuarioService.GetType()
                .GetMethod("ObtenerIdSesion", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .Invoke(_usuarioService, null);

            Assert.Equal(1, result);
        }

        [Fact]
        public void ObtenerIdSesion_ClaimNoConvertibleAInt()
        {
            var claims = new List<Claim> { new Claim("UsuarioId", "abc") };
            var identity = new ClaimsIdentity(claims, "mock");
            var principal = new ClaimsPrincipal(identity);
            _mockHttpContext.Setup(h => h.HttpContext).Returns(new DefaultHttpContext { User = principal });

            var result = _usuarioService.GetType()
                .GetMethod("ObtenerIdSesion", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .Invoke(_usuarioService, null);

            Assert.Equal(1, result);
        }

        // ObtenerTodos
        [Fact]
        public async Task ObtenerTodos_RetornaSoloActivos()
        {
            var usuarios = new List<Usuario>
            {
                new Usuario { Id = 1, Activo = 1 },
                new Usuario { Id = 2, Activo = 0 }
            }.BuildMockAsyncQueryable();

            _mockUsuarioRepo.Setup(r => r.ObtenerTodos()).Returns(usuarios);

            var result = await _usuarioService.ObtenerTodos();

            Assert.Single(result);
            Assert.Equal(1, result.First().Id);
        }

        // ObtenerPorId
        [Fact]
        public async Task ObtenerPorId_UsuarioExiste()
        {
            var usuario = new Usuario { Id = 10 };
            _mockUsuarioRepo.Setup(r => r.ObtenerPorId(10)).ReturnsAsync(usuario);

            var result = await _usuarioService.ObtenerPorId(10);

            Assert.NotNull(result);
            Assert.Equal(10, result!.Id);
        }

        [Fact]
        public async Task ObtenerPorId_UsuarioNoExiste()
        {
            _mockUsuarioRepo.Setup(r => r.ObtenerPorId(20)).ReturnsAsync((Usuario?)null);

            var result = await _usuarioService.ObtenerPorId(20);

            Assert.Null(result);
        }

        // Crear
        [Fact]
        public async Task Crear_UsuarioConContrasenia_Hasheada()
        {
            var usuario = new Usuario { Id = 1, Contrasenia = "1234" };

            var result = await _usuarioService.Crear(usuario);

            Assert.NotNull(result.Contrasenia);
            Assert.NotEqual("1234", result.Contrasenia); // se debe hashear
            Assert.Equal(1, result.Activo);
            _mockUsuarioRepo.Verify(r => r.Agregar(It.IsAny<Usuario>()), Times.Once);
        }

        [Fact]
        public async Task Crear_UsuarioSinContrasenia_NoHashea()
        {
            var usuario = new Usuario { Id = 2 };

            var result = await _usuarioService.Crear(usuario);

            Assert.Null(result.Contrasenia);
            Assert.Equal(1, result.Activo);
            _mockUsuarioRepo.Verify(r => r.Agregar(It.IsAny<Usuario>()), Times.Once);
        }

        // Actualizar
        [Fact]
        public async Task Actualizar_UsuarioExiste_SeActualiza()
        {
            var existente = new Usuario { Id = 1, FechaCreacion = DateTime.Now.AddDays(-2), CreadoPor = 10, Activo = 1, Rol = "Admin" };
            _mockUsuarioRepo.Setup(r => r.ObtenerPorId(1)).ReturnsAsync(existente);

            var modificado = new Usuario { Id = 1, PrimerNombre = "Nuevo" };

            var result = await _usuarioService.Actualizar(modificado);

            Assert.Equal(existente.FechaCreacion, result.FechaCreacion);
            Assert.Equal("Admin", result.Rol);
            Assert.NotNull(result.UltimaActualizacion);
            _mockUsuarioRepo.Verify(r => r.Actualizar(It.IsAny<Usuario>()), Times.Once);
        }

        [Fact]
        public async Task Actualizar_UsuarioNoExiste_LanzaExcepcion()
        {
            _mockUsuarioRepo.Setup(r => r.ObtenerPorId(5)).ReturnsAsync((Usuario?)null);

            var usuario = new Usuario { Id = 5 };

            await Assert.ThrowsAsync<Exception>(() => _usuarioService.Actualizar(usuario));
        }

        // Eliminar
        [Fact]
        public async Task Eliminar_UsuarioExiste()
        {
            var usuario = new Usuario { Id = 3, Activo = 1 };
            _mockUsuarioRepo.Setup(r => r.ObtenerPorId(3)).ReturnsAsync(usuario);

            var result = await _usuarioService.Eliminar(3);

            Assert.True(result);
            Assert.Equal(0, usuario.Activo);
            _mockUsuarioRepo.Verify(r => r.Actualizar(It.Is<Usuario>(u => u.Activo == 0)), Times.Once);
        }

        [Fact]
        public async Task Eliminar_UsuarioNoExiste()
        {
            _mockUsuarioRepo.Setup(r => r.ObtenerPorId(99)).ReturnsAsync((Usuario?)null);

            var result = await _usuarioService.Eliminar(99);

            Assert.False(result);
            _mockUsuarioRepo.Verify(r => r.Actualizar(It.IsAny<Usuario>()), Times.Never);
        }
    }
}
