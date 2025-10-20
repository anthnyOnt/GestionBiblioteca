using GestionBiblioteca.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GestionBiblioteca.Services.Usuario;

public class UsuarioService: IUsuarioService
{
    private readonly IRepositoryFactory _repositoryFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly PasswordHasher<Entities.Usuario> _hasher;
    public UsuarioService(IRepositoryFactory repositoryFactory, IHttpContextAccessor httpContextAccessor)
    {
        _repositoryFactory = repositoryFactory;
        _httpContextAccessor = httpContextAccessor;
        _hasher = new PasswordHasher<Entities.Usuario>();
    }

    private int ObtenerIdSesion()
    {
        if (_httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true)
        {
            var idResponsable = _httpContextAccessor.HttpContext.User.FindFirst("UsuarioId")?.Value;
            if (int.TryParse(idResponsable, out int id))
            {
                return id;
            }
        }

        return 1;
    }
    
    public async Task<List<Entities.Usuario>> ObtenerTodos()
    {
        return await _repositoryFactory.ObtenerRepository<Entities.Usuario>().ObtenerPorConsulta()
            .Where(u => u.Activo.Equals(1))
            .ToListAsync();
    }

    public async Task<Entities.Usuario?> ObtenerPorId(int id)
    {
        return await _repositoryFactory.ObtenerRepository<Entities.Usuario>().ObtenerPorId(id);
    }

    public async Task<Entities.Usuario> ObtenerPorCi(string ci)
    {
        return await _repositoryFactory.ObtenerRepository<Entities.Usuario>().ObtenerPorConsulta()
            .Where(u => u.Activo.Equals(1) && u.Ci.Equals(ci))
            .FirstOrDefaultAsync();
    }

    public async Task<Entities.Usuario> Crear(Entities.Usuario usuario)
    {
        usuario.Activo = 1;
        usuario.CreadoPor = ObtenerIdSesion();
        usuario.FechaCreacion = DateTime.Now;
        if (usuario.Contrasenia != null)
        {
            usuario.Contrasenia = _hasher.HashPassword(null, usuario.Contrasenia);
        }
        await _repositoryFactory.ObtenerRepository<Entities.Usuario>().Agregar(usuario);
        return usuario;
    }

    public async Task<Entities.Usuario?> Actualizar(Entities.Usuario usuario)
    {
        var repo = _repositoryFactory.ObtenerRepository<Entities.Usuario>();
        var existing = await repo.ObtenerPorId(usuario.Id);

        if (existing == null)
            throw new Exception("Usuario not found");

        usuario.FechaCreacion = existing.FechaCreacion;
        usuario.CreadoPor = existing.CreadoPor;
        usuario.Activo = existing.Activo;
        usuario.Rol = existing.Rol;
        
        usuario.UltimaActualizacion = DateTime.Now;
        await repo.Actualizar(usuario);
        return usuario;
    }

    public async Task<bool> Eliminar(int id)
    {
        Entities.Usuario usuario =  await _repositoryFactory.ObtenerRepository<Entities.Usuario>().ObtenerPorId(id);
        if (usuario != null)
        {
            usuario.Activo = 0;
            await _repositoryFactory.ObtenerRepository<Entities.Usuario>().Actualizar(usuario);
            return true;
        }
        return false;
    }

    public async Task<Entities.Usuario> AgregarLector(Entities.Usuario lector)
    {
        lector.Activo = 1;
        lector.CreadoPor = ObtenerIdSesion();
        lector.FechaCreacion = DateTime.Now;
        lector.Rol = "LECTOR";
        await _repositoryFactory.ObtenerRepository<Entities.Usuario>().Agregar(lector);
        return lector;
    }
}