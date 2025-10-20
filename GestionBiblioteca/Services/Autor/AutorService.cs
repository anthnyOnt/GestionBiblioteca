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

    public async Task<List<Entities.Autor>> ObtenerTodos()
    {
        return await _repositoryFactory.ObtenerRepository<Entities.Autor>().ObtenerPorConsulta()
            .ToListAsync();
    }

    public async Task<Entities.Autor?> ObtenerPorId(int id)
    {
        return await _repositoryFactory.ObtenerRepository<Entities.Autor>().ObtenerPorId(id);
    }

    public async Task<Entities.Autor> Crear(Entities.Autor autor)
    {
        await _repositoryFactory.ObtenerRepository<Entities.Autor>().Agregar(autor);
        return autor;
    }

    public async Task<Entities.Autor?> Actualizar(Entities.Autor autor)
    {
        var repo = _repositoryFactory.ObtenerRepository<Entities.Autor>();
        var existing = await repo.ObtenerPorId(autor.Id);

        if (existing == null)
            throw new Exception("Autor not found");

        await repo.Actualizar(autor);
        return autor;
    }

    public async Task<bool> Eliminar(int id)
    {
        Entities.Autor autor =  await _repositoryFactory.ObtenerRepository<Entities.Autor>().ObtenerPorId(id);
        if (autor != null)
        {
            await _repositoryFactory.ObtenerRepository<Entities.Autor>().Actualizar(autor);
            return true;
        }
        return false;
    }
}