using GestionBiblioteca.Entities;
using GestionBiblioteca.Repository;
using GestionBiblioteca.Services.Libro;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using Xunit;
using GestionBibliotecaTests.Helpers;

namespace GestionBibliotecaTests
{
    public class LibroServiceTest
    {
        private readonly Mock<IRepositoryFactory> _mockRepoFactory;
        private readonly Mock<IRepository<Libro>> _mockLibroRepo;
        private readonly Mock<IHttpContextAccessor> _mockHttpContext;
        private readonly LibroService _libroService;

        public LibroServiceTest()
        {
            _mockRepoFactory = new Mock<IRepositoryFactory>();
            _mockLibroRepo = new Mock<IRepository<Libro>>();
            _mockHttpContext = new Mock<IHttpContextAccessor>();

            _mockRepoFactory.Setup(f => f.ObtenerRepository<Libro>())
                .Returns(_mockLibroRepo.Object);

            _libroService = new LibroService(_mockRepoFactory.Object, _mockHttpContext.Object);
        }

        // ObtenerIdSesion
        [Fact]
        public void ObtenerIdSesion_UsuarioAutenticadoConClaimValida()
        {
            var claims = new List<Claim> { new Claim("LibroId", "5") };
            var identity = new ClaimsIdentity(claims, "mock");
            var principal = new ClaimsPrincipal(identity);
            _mockHttpContext.Setup(h => h.HttpContext).Returns(new DefaultHttpContext { User = principal });

            var result = _libroService.GetType()
                .GetMethod("ObtenerIdSesion", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .Invoke(_libroService, null);

            Assert.Equal(5, result);
        }

        [Fact]
        public void ObtenerIdSesion_UsuarioAutenticadoSinClaim()
        {
            var identity = new ClaimsIdentity(new List<Claim>(), "mock");
            var principal = new ClaimsPrincipal(identity);
            _mockHttpContext.Setup(h => h.HttpContext).Returns(new DefaultHttpContext { User = principal });

            var result = _libroService.GetType()
                .GetMethod("ObtenerIdSesion", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .Invoke(_libroService, null);

            Assert.Equal(1, result);
        }

        [Fact]
        public void ObtenerIdSesion_UsuarioNoAutenticado()
        {
            _mockHttpContext.Setup(h => h.HttpContext).Returns(new DefaultHttpContext());

            var result = _libroService.GetType()
                .GetMethod("ObtenerIdSesion", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .Invoke(_libroService, null);

            Assert.Equal(1, result);
        }

        [Fact]
        public void ObtenerIdSesion_ClaimNoConvertibleAInt()
        {
            var claims = new List<Claim> { new Claim("LibroId", "abc") };
            var identity = new ClaimsIdentity(claims, "mock");
            var principal = new ClaimsPrincipal(identity);
            _mockHttpContext.Setup(h => h.HttpContext).Returns(new DefaultHttpContext { User = principal });

            var result = _libroService.GetType()
                .GetMethod("ObtenerIdSesion", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .Invoke(_libroService, null);

            Assert.Equal(1, result);
        }

        // ObtenerTodos
        [Fact]
        public async Task ObtenerTodos_RetornaTodos()
        {
            // Arrange
            var libros = new List<Libro>
            {
                new Libro { Id = 1, Titulo = "Libro 1" },
                new Libro { Id = 2, Titulo = "Libro 2" }
            };
            
            var mockDbSet = libros.AsQueryable().BuildMockAsyncQueryable();
            _mockLibroRepo.Setup(r => r.ObtenerPorConsulta()).Returns(mockDbSet);

            // Act
            var result = await _libroService.ObtenerTodos();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, l => l.Id == 1 && l.Titulo == "Libro 1");
            Assert.Contains(result, l => l.Id == 2 && l.Titulo == "Libro 2");
        }

        // ObtenerPorId
        [Fact]
        public async Task ObtenerPorId_LibroExiste()
        {
            var libro = new Libro { Id = 10, Titulo = "Libro de Prueba" };
            _mockLibroRepo.Setup(r => r.ObtenerPorId(10)).ReturnsAsync(libro);

            var result = await _libroService.ObtenerPorId(10);

            Assert.NotNull(result);
            Assert.Equal(10, result!.Id);
            Assert.Equal("Libro de Prueba", result!.Titulo);
        }

        [Fact]
        public async Task ObtenerPorId_LibroNoExiste()
        {
            _mockLibroRepo.Setup(r => r.ObtenerPorId(20)).ReturnsAsync((Libro?)null);

            var result = await _libroService.ObtenerPorId(20);

            Assert.Null(result);
        }

        // ObtenerEjemplaresPorTitulo
        [Fact]
        public async Task ObtenerEjemplaresPorTitulo_RetornaLibrosConEjemplaresDisponibles()
        {
            // Arrange
            var ejemplaresLibro1 = new List<Ejemplar> { 
                new Ejemplar { Id = 1, Disponible = true },
                new Ejemplar { Id = 2, Disponible = true },
                new Ejemplar { Id = 3, Disponible = false }
            };
            
            var ejemplaresLibro2 = new List<Ejemplar> { 
                new Ejemplar { Id = 4, Disponible = false }
            };

            var libros = new List<Libro>
            {
                new Libro { Id = 1, Titulo = "Harry Potter", Ejemplares = ejemplaresLibro1 },
                new Libro { Id = 2, Titulo = "Harry Potter y la piedra filosofal", Ejemplares = ejemplaresLibro2 }
            };

            var mockDbSet = libros.AsQueryable().BuildMockAsyncQueryable();
            _mockLibroRepo.Setup(r => r.ObtenerPorConsulta()).Returns(mockDbSet);

            // Act
            var result = await _libroService.ObtenerEjemplaresPorTitulo("Harry");

            // Assert
            Assert.Equal(2, result.Count);
            
            var libro1 = result.FirstOrDefault(l => l.Id == 1);
            Assert.NotNull(libro1);
            Assert.Equal(2, libro1.Ejemplares.Count);
            Assert.All(libro1.Ejemplares, e => Assert.True(e.Disponible));
            
            var libro2 = result.FirstOrDefault(l => l.Id == 2);
            Assert.NotNull(libro2);
            Assert.Empty(libro2.Ejemplares);
        }

        // Crear
        [Fact]
        public async Task Crear_NuevoLibro()
        {
            var libro = new Libro { Id = 1, Titulo = "Nuevo Libro" };

            var result = await _libroService.Crear(libro);

            Assert.Equal("Nuevo Libro", result.Titulo);
            _mockLibroRepo.Verify(r => r.Agregar(It.IsAny<Libro>()), Times.Once);
        }

        // Actualizar
        [Fact]
        public async Task Actualizar_LibroExiste_SeActualiza()
        {
            var existente = new Libro { Id = 1, Titulo = "Título Original" };
            _mockLibroRepo.Setup(r => r.ObtenerPorId(1)).ReturnsAsync(existente);

            var modificado = new Libro { Id = 1, Titulo = "Título Nuevo" };

            var result = await _libroService.Actualizar(modificado);

            Assert.Equal("Título Nuevo", result!.Titulo);
            _mockLibroRepo.Verify(r => r.Actualizar(It.IsAny<Libro>()), Times.Once);
        }

        [Fact]
        public async Task Actualizar_LibroNoExiste_LanzaExcepcion()
        {
            _mockLibroRepo.Setup(r => r.ObtenerPorId(5)).ReturnsAsync((Libro?)null);

            var libro = new Libro { Id = 5, Titulo = "No Existe" };

            await Assert.ThrowsAsync<Exception>(() => _libroService.Actualizar(libro));
        }

        // Eliminar
        [Fact]
        public async Task Eliminar_LibroExiste()
        {
            var libro = new Libro { Id = 3 };
            _mockLibroRepo.Setup(r => r.ObtenerPorId(3)).ReturnsAsync(libro);

            var result = await _libroService.Eliminar(3);

            Assert.True(result);
            _mockLibroRepo.Verify(r => r.Actualizar(It.IsAny<Libro>()), Times.Once);
        }

        [Fact]
        public async Task Eliminar_LibroNoExiste()
        {
            _mockLibroRepo.Setup(r => r.ObtenerPorId(99)).ReturnsAsync((Libro?)null);

            var result = await _libroService.Eliminar(99);

            Assert.False(result);
            _mockLibroRepo.Verify(r => r.Actualizar(It.IsAny<Libro>()), Times.Never);
        }
    }
}