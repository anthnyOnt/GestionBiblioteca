using GestionBiblioteca.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GestionBiblioteca.Services.Editorial;

public class EditorialService: IEditorialService
{
    private readonly IRepositoryFactory _repositoryFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly PasswordHasher<Entities.Editorial> _hasher;

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
    
    public async Task<List<Entities.Editorial>> ObtenerTodos()
    {
        return await _repositoryFactory.ObtenerRepository<Entities.Editorial>().ObtenerPorConsulta()
            .ToListAsync();
    }

    public async Task<Entities.Editorial?> ObtenerPorId(int id)
    {
        return await _repositoryFactory.ObtenerRepository<Entities.Editorial>().ObtenerPorId(id);
    }

    public async Task<Entities.Editorial> Crear(Entities.Editorial editorial)
    {
        await _repositoryFactory.ObtenerRepository<Entities.Editorial>().Agregar(editorial);
        return editorial;
    }

    public async Task<Entities.Editorial?> Actualizar(Entities.Editorial editorial)
    {
        var repo = _repositoryFactory.ObtenerRepository<Entities.Editorial>();
        var existing = await repo.ObtenerPorId(editorial.Id);

        if (existing == null)
            throw new Exception("Editorial not found");

        await repo.Actualizar(editorial);
        return editorial;
    }

    public async Task<bool> Eliminar(int id)
    {
        Entities.Editorial editorial =  await _repositoryFactory.ObtenerRepository<Entities.Editorial>().ObtenerPorId(id);
        if (editorial != null)
        {
            await _repositoryFactory.ObtenerRepository<Entities.Editorial>().Actualizar(editorial);
            return true;
        }
        return false;
    }
}