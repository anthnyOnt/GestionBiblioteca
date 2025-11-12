using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using GestionBiblioteca.Entities;
using GestionBibliotecaTestsIntegracion.Config;
using GestionBiblioteca.Services.Usuario;
using GestionBiblioteca.Context;
using Microsoft.Extensions.DependencyInjection;

namespace GestionBibliotecaTestsIntegracion.Persistence
{
    public class UsuarioPersistenceTests
    {
        private readonly IServiceProvider _sp;
        private IUsuarioService UsuarioSvc => _sp.GetRequiredService<IUsuarioService>();
        private MyDbContext Ctx => _sp.GetRequiredService<MyDbContext>();

        public UsuarioPersistenceTests()
        {
            _sp = ServiceTestHelper.BuildProvider();
        }

        [Fact]
        public async Task PER_007_Insertar_Usuario()
        {
            var u = await UsuarioSvc.AgregarLector(new Usuario
            {
                PrimerNombre = "Test",
                PrimerApellido = "Persistencia",
                Correo = "test.persistencia@ucb.edu.bo",
                Contrasenia = "pwd"
            });

            var ok = await Ctx.Usuarios.AnyAsync(x => x.Id == u.Id && x.Correo == "test.persistencia@ucb.edu.bo");
            Assert.True(ok);
        }

        [Fact]
        public async Task PER_008_Actualizar_Usuario()
        {
            var u = await UsuarioSvc.AgregarLector(new Usuario
            {
                PrimerNombre = "Tmp",
                PrimerApellido = "Usr",
                Correo = "tmp@ucb.edu.bo",
                Contrasenia = "pwd"
            });

            u.PrimerNombre = "TmpEditado";
            await UsuarioSvc.Actualizar(u);

            var check = await Ctx.Usuarios.FindAsync(u.Id);
            Assert.Equal("TmpEditado", check!.PrimerNombre);
        }

        [Fact]
        public async Task PER_009_Eliminar_Usuario()
        {
            var u = await UsuarioSvc.AgregarLector(new Usuario
            {
                PrimerNombre = "Borrar",
                PrimerApellido = "Usr",
                Correo = "borrar@ucb.edu.bo",
                Contrasenia = "pwd"
            });

            var okDelete = await UsuarioSvc.Eliminar(u.Id);
            Assert.True(okDelete);

            var gone = await Ctx.Usuarios.FindAsync(u.Id);
            Assert.Equal(0, gone!.Activo); // borrado lógico
        }
    }
}
