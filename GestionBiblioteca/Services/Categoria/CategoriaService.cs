using GestionBiblioteca.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GestionBiblioteca.Services.Categoria;

public class CategoriaService: ICategoriaService
{
    private readonly IRepositoryFactory _repositoryFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly PasswordHasher<Entities.Categoria> _hasher;
    public CategoriaService(IRepositoryFactory repositoryFactory, IHttpContextAccessor httpContextAccessor)
    {
        _repositoryFactory = repositoryFactory;
        _httpContextAccessor = httpContextAccessor;
        _hasher = new PasswordHasher<Entities.Categoria>();
    }

    private int ObtenerIdSesion()
    {
        if (_httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true)
        {
            var idResponsable = _httpContextAccessor.HttpContext.User.FindFirst("CategoriaId")?.Value;
            if (int.TryParse(idResponsable, out int id))
            {
                return id;
            }
        }

        return 1;
    }

    public async Task<List<Entities.Categoria>> ObtenerTodos()
    {
        return await _repositoryFactory.ObtenerRepository<Entities.Categoria>().ObtenerPorConsulta()
            .ToListAsync();
    }

    public async Task<Entities.Categoria?> ObtenerPorId(int id)
    {
        return await _repositoryFactory.ObtenerRepository<Entities.Categoria>().ObtenerPorId(id);
    }

    public async Task<Entities.Categoria> Crear(Entities.Categoria categoria)
    {
        await _repositoryFactory.ObtenerRepository<Entities.Categoria>().Agregar(categoria);
        return categoria;
    }

    public async Task<Entities.Categoria?> Actualizar(Entities.Categoria categoria)
    {
        var repo = _repositoryFactory.ObtenerRepository<Entities.Categoria>();
        var existing = await repo.ObtenerPorId(categoria.Id);

        if (existing == null)
            throw new Exception("Categoria not found");

        await repo.Actualizar(categoria);
        return categoria;
    }

    public async Task<bool> Eliminar(int id)
    {
        Entities.Categoria categoria =  await _repositoryFactory.ObtenerRepository<Entities.Categoria>().ObtenerPorId(id);
        if (categoria != null)
        {
            await _repositoryFactory.ObtenerRepository<Entities.Categoria>().Actualizar(categoria);
            return true;
        }
        return false;
    }
}