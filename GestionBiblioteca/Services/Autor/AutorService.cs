using GestionBiblioteca.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GestionBiblioteca.Services.Autor;

public class AutorService: IAutorService
{
    private readonly IRepositoryFactory _repositoryFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly PasswordHasher<Entities.Autor> _hasher;
    //password hasher
    public AutorService(IRepositoryFactory repositoryFactory, IHttpContextAccessor httpContextAccessor)
    {
        _repositoryFactory = repositoryFactory;
        _httpContextAccessor = httpContextAccessor;
        _hasher = new PasswordHasher<Entities.Autor>();
    }

    private int ObtenerIdSesion()
    {
        if (_httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true)
        {
            var idResponsable = _httpContextAccessor.HttpContext.User.FindFirst("AutorId")?.Value;
            if (int.TryParse(idResponsable, out int id))
            {
                return id;
            }
        }

        return 1;
    }

    // public async Task<List<Entities.Autor>> ObtenerTodos()
    // {
    //     var Autors = await _repositoryFactory.ObtenerRepository<Entities.Autor>().ObtenerTodos();
    //     List<Entities.Autor> lista = new List<Entities.Autor>();
    //     foreach (var Autor in Autors)
    //     {
    //         if (Autor.Activo == 1)
    //             lista.Add(Autor);
    //     }
    //
    //     return lista;
    // }
    
    public async Task<List<Entities.Autor>> ObtenerTodos()
    {
        return await _repositoryFactory.ObtenerRepository<Entities.Autor>().ObtenerTodos()
            // .Where(u => u.Activo.Equals(1))
            .ToListAsync();
    }

    public async Task<Entities.Autor?> ObtenerPorId(int id)
    {
        return await _repositoryFactory.ObtenerRepository<Entities.Autor>().ObtenerPorId(id);
    }

    public async Task<Entities.Autor> Crear(Entities.Autor autor)
    {
        // Autor.Activo = 1;
        // Autor.CreadoPor = ObtenerIdSesion();
        // Autor.FechaCreacion = DateTime.Now;
        await _repositoryFactory.ObtenerRepository<Entities.Autor>().Agregar(autor);
        return autor;
    }

    public async Task<Entities.Autor?> Actualizar(Entities.Autor autor)
    {
        var repo = _repositoryFactory.ObtenerRepository<Entities.Autor>();
        var existing = await repo.ObtenerPorId(autor.Id);

        if (existing == null)
            throw new Exception("Autor not found");

        // Autor.FechaCreacion = existing.FechaCreacion;
        // Autor.CreadoPor = existing.CreadoPor;
        // Autor.Activo = existing.Activo;
        // Autor.Rol = existing.Rol;
        //
        // Autor.UltimaActualizacion = DateTime.Now;
        await repo.Actualizar(autor);
        return autor;
    }

    public async Task<bool> Eliminar(int id)
    {
        Entities.Autor autor =  await _repositoryFactory.ObtenerRepository<Entities.Autor>().ObtenerPorId(id);
        if (autor != null)
        {
            // Autor.Activo = 0;
            await _repositoryFactory.ObtenerRepository<Entities.Autor>().Actualizar(autor);
            return true;
        }
        return false;
    }
}