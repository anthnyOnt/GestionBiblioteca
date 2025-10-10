using GestionBiblioteca.Entities;
using GestionBiblioteca.Tests.Helpers;

namespace GestionBibliotecaTests.EntitiesTests
{
    public class UsuarioTests
    {
        private Usuario CrearUsuarioValido() => new Usuario
        {
            PrimerNombre = "Diego",
            SegundoNombre = "Andr�s",
            PrimerApellido = "Valdez",
            SegundoApellido = "P�rez",
            Ci = "1234567",
            Telefono = "76543210",
            Correo = "diego@correo.com",
            Contrasenia = "12345",
            Rol = "Admin",
            Activo = 1
        };

        //PrimerNombre
        [Fact]
        public void Usuario_SinPrimerNombre_GeneraError()
        {
            var usuario = CrearUsuarioValido();
            usuario.PrimerNombre = "";
            var result = ValidationHelper.Validate(usuario);
            Assert.Contains(result, r => r.MemberNames.Contains("PrimerNombre"));
        }

        [Fact]
        public void Usuario_PrimerNombreConNumeros_GeneraError()
        {
            var usuario = CrearUsuarioValido();
            usuario.PrimerNombre = "Juan123";
            var result = ValidationHelper.Validate(usuario);
            Assert.Contains(result, r => r.MemberNames.Contains("PrimerNombre"));
        }

        [Fact]
        public void Usuario_PrimerNombreMuyCorto_GeneraError()
        {
            var usuario = CrearUsuarioValido();
            usuario.PrimerNombre = "Jo";
            var result = ValidationHelper.Validate(usuario);
            Assert.Contains(result, r => r.MemberNames.Contains("PrimerNombre"));
        }

        [Fact]
        public void Usuario_PrimerNombreMuyLargo_GeneraError()
        {
            var usuario = CrearUsuarioValido();
            usuario.PrimerNombre = new string('a', 31);
            var result = ValidationHelper.Validate(usuario);
            Assert.Contains(result, r => r.MemberNames.Contains("PrimerNombre"));
        }

        // SegundoNombre
        [Fact]
        public void Usuario_SegundoNombreConNumeros_GeneraError()
        {
            var usuario = CrearUsuarioValido();
            usuario.SegundoNombre = "Andres123";
            var result = ValidationHelper.Validate(usuario);
            Assert.Contains(result, r => r.MemberNames.Contains("SegundoNombre"));
        }

        [Fact]
        public void Usuario_SegundoNombreMuyLargo_GeneraError()
        {
            var usuario = CrearUsuarioValido();
            usuario.SegundoNombre = new string('a', 31);
            var result = ValidationHelper.Validate(usuario);
            Assert.Contains(result, r => r.MemberNames.Contains("SegundoNombre"));
        }

        // PrimerApellido
        [Fact]
        public void Usuario_SinPrimerApellido_GeneraError()
        {
            var usuario = CrearUsuarioValido();
            usuario.PrimerApellido = "";
            var result = ValidationHelper.Validate(usuario);
            Assert.Contains(result, r => r.MemberNames.Contains("PrimerApellido"));
        }

        // Ci
        [Fact]
        public void Usuario_SinCi_GeneraError()
        {
            var usuario = CrearUsuarioValido();
            usuario.Ci = "";
            var result = ValidationHelper.Validate(usuario);
            Assert.Contains(result, r => r.MemberNames.Contains("Ci"));
        }

        [Fact]
        public void Usuario_CiConLetras_GeneraError()
        {
            var usuario = CrearUsuarioValido();
            usuario.Ci = "ABC123";
            var result = ValidationHelper.Validate(usuario);
            Assert.Contains(result, r => r.MemberNames.Contains("Ci"));
        }

        [Fact]
        public void Usuario_CiMuyCorto_GeneraError()
        {
            var usuario = CrearUsuarioValido();
            usuario.Ci = "123";
            var result = ValidationHelper.Validate(usuario);
            Assert.Contains(result, r => r.MemberNames.Contains("Ci"));
        }

        // Telefono
        [Fact]
        public void Usuario_SinTelefono_GeneraError()
        {
            var usuario = CrearUsuarioValido();
            usuario.Telefono = "";
            var result = ValidationHelper.Validate(usuario);
            Assert.Contains(result, r => r.MemberNames.Contains("Telefono"));
        }

        [Fact]
        public void Usuario_TelefonoConLetras_GeneraError()
        {
            var usuario = CrearUsuarioValido();
            usuario.Telefono = "abc123";
            var result = ValidationHelper.Validate(usuario);
            Assert.Contains(result, r => r.MemberNames.Contains("Telefono"));
        }

        [Fact]
        public void Usuario_TelefonoMuyCorto_GeneraError()
        {
            var usuario = CrearUsuarioValido();
            usuario.Telefono = "12345";
            var result = ValidationHelper.Validate(usuario);
            Assert.Contains(result, r => r.MemberNames.Contains("Telefono"));
        }

        //Correo
        [Fact]
        public void Usuario_SinCorreo_GeneraError()
        {
            var usuario = CrearUsuarioValido();
            usuario.Correo = "";
            var result = ValidationHelper.Validate(usuario);
            Assert.Contains(result, r => r.MemberNames.Contains("Correo"));
        }

        [Fact]
        public void Usuario_CorreoInvalido_GeneraError()
        {
            var usuario = CrearUsuarioValido();
            usuario.Correo = "correo_invalido";
            var result = ValidationHelper.Validate(usuario);
            Assert.Contains(result, r => r.MemberNames.Contains("Correo"));
        }

        [Fact]
        public void Usuario_CorreoMuyLargo_GeneraError()
        {
            var usuario = CrearUsuarioValido();
            usuario.Correo = new string('a', 46) + "@mail.com";
            var result = ValidationHelper.Validate(usuario);
            Assert.Contains(result, r => r.MemberNames.Contains("Correo"));
        }
    }
}
