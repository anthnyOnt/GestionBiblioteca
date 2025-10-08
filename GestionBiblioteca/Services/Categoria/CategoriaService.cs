using GestionBiblioteca.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GestionBiblioteca.Services.Categoria;

public class CategoriaService: ICategoriaService
{
    private readonly IRepositoryFactory _repositoryFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly PasswordHasher<Entities.Categoria> _hasher;
    //password hasher
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

    // public async Task<List<Entities.Categoria>> ObtenerTodos()
    // {
    //     var Categorias = await _repositoryFactory.ObtenerRepository<Entities.Categoria>().ObtenerTodos();
    //     List<Entities.Categoria> lista = new List<Entities.Categoria>();
    //     foreach (var Categoria in Categorias)
    //     {
    //         if (Categoria.Activo == 1)
    //             lista.Add(Categoria);
    //     }
    //
    //     return lista;
    // }
    
    public async Task<List<Entities.Categoria>> ObtenerTodos()
    {
        return await _repositoryFactory.ObtenerRepository<Entities.Categoria>().ObtenerPorConsulta()
            // .Where(u => u.Activo.Equals(1))
            .ToListAsync();
    }

    public async Task<Entities.Categoria?> ObtenerPorId(int id)
    {
        return await _repositoryFactory.ObtenerRepository<Entities.Categoria>().ObtenerPorId(id);
    }

    public async Task<Entities.Categoria> Crear(Entities.Categoria categoria)
    {
        // Categoria.Activo = 1;
        // Categoria.CreadoPor = ObtenerIdSesion();
        // Categoria.FechaCreacion = DateTime.Now;
        await _repositoryFactory.ObtenerRepository<Entities.Categoria>().Agregar(categoria);
        return categoria;
    }

    public async Task<Entities.Categoria?> Actualizar(Entities.Categoria categoria)
    {
        var repo = _repositoryFactory.ObtenerRepository<Entities.Categoria>();
        var existing = await repo.ObtenerPorId(categoria.Id);

        if (existing == null)
            throw new Exception("Categoria not found");

        // Categoria.FechaCreacion = existing.FechaCreacion;
        // Categoria.CreadoPor = existing.CreadoPor;
        // Categoria.Activo = existing.Activo;
        // Categoria.Rol = existing.Rol;
        //
        // Categoria.UltimaActualizacion = DateTime.Now;
        await repo.Actualizar(categoria);
        return categoria;
    }

    public async Task<bool> Eliminar(int id)
    {
        Entities.Categoria categoria =  await _repositoryFactory.ObtenerRepository<Entities.Categoria>().ObtenerPorId(id);
        if (categoria != null)
        {
            // Categoria.Activo = 0;
            await _repositoryFactory.ObtenerRepository<Entities.Categoria>().Actualizar(categoria);
            return true;
        }
        return false;
    }
}