using GestionBiblioteca.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GestionBiblioteca.Services.Editorial;

public class EditorialService: IEditorialService
{
    private readonly IRepositoryFactory _repositoryFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly PasswordHasher<Entities.Editorial> _hasher;
    //password hasher
    public EditorialService(IRepositoryFactory repositoryFactory, IHttpContextAccessor httpContextAccessor)
    {
        _repositoryFactory = repositoryFactory;
        _httpContextAccessor = httpContextAccessor;
        _hasher = new PasswordHasher<Entities.Editorial>();
    }

    private int ObtenerIdSesion()
    {
        if (_httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true)
        {
            var idResponsable = _httpContextAccessor.HttpContext.User.FindFirst("EditorialId")?.Value;
            if (int.TryParse(idResponsable, out int id))
            {
                return id;
            }
        }

        return 1;
    }

    // public async Task<List<Entities.Editorial>> ObtenerTodos()
    // {
    //     var Editorials = await _repositoryFactory.ObtenerRepository<Entities.Editorial>().ObtenerTodos();
    //     List<Entities.Editorial> lista = new List<Entities.Editorial>();
    //     foreach (var Editorial in Editorials)
    //     {
    //         if (Editorial.Activo == 1)
    //             lista.Add(Editorial);
    //     }
    //
    //     return lista;
    // }
    
    public async Task<List<Entities.Editorial>> ObtenerTodos()
    {
        return await _repositoryFactory.ObtenerRepository<Entities.Editorial>().ObtenerTodos()
            // .Where(u => u.Activo.Equals(1))
            .ToListAsync();
    }

    public async Task<Entities.Editorial?> ObtenerPorId(int id)
    {
        return await _repositoryFactory.ObtenerRepository<Entities.Editorial>().ObtenerPorId(id);
    }

    public async Task<Entities.Editorial> Crear(Entities.Editorial editorial)
    {
        // Editorial.Activo = 1;
        // Editorial.CreadoPor = ObtenerIdSesion();
        // Editorial.FechaCreacion = DateTime.Now;
        await _repositoryFactory.ObtenerRepository<Entities.Editorial>().Agregar(editorial);
        return editorial;
    }

    public async Task<Entities.Editorial?> Actualizar(Entities.Editorial editorial)
    {
        var repo = _repositoryFactory.ObtenerRepository<Entities.Editorial>();
        var existing = await repo.ObtenerPorId(editorial.Id);

        if (existing == null)
            throw new Exception("Editorial not found");

        // Editorial.FechaCreacion = existing.FechaCreacion;
        // Editorial.CreadoPor = existing.CreadoPor;
        // Editorial.Activo = existing.Activo;
        // Editorial.Rol = existing.Rol;
        //
        // Editorial.UltimaActualizacion = DateTime.Now;
        await repo.Actualizar(editorial);
        return editorial;
    }

    public async Task<bool> Eliminar(int id)
    {
        Entities.Editorial editorial =  await _repositoryFactory.ObtenerRepository<Entities.Editorial>().ObtenerPorId(id);
        if (editorial != null)
        {
            // Editorial.Activo = 0;
            await _repositoryFactory.ObtenerRepository<Entities.Editorial>().Actualizar(editorial);
            return true;
        }
        return false;
    }
}