using GestionBiblioteca.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GestionBiblioteca.Services.Libro;

public class LibroService: ILibroService
{
    private readonly IRepositoryFactory _repositoryFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly PasswordHasher<Entities.Libro> _hasher;
    public LibroService(IRepositoryFactory repositoryFactory, IHttpContextAccessor httpContextAccessor)
    {
        _repositoryFactory = repositoryFactory;
        _httpContextAccessor = httpContextAccessor;
        _hasher = new PasswordHasher<Entities.Libro>();
    }

    private int ObtenerIdSesion()
    {
        if (_httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true)
        {
            var idResponsable = _httpContextAccessor.HttpContext.User.FindFirst("LibroId")?.Value;
            if (int.TryParse(idResponsable, out int id))
            {
                return id;
            }
        }

        return 1;
    }
    
    public async Task<List<Entities.Libro>> ObtenerTodos()
    {
        
        return await _repositoryFactory.ObtenerRepository<Entities.Libro>().ObtenerPorConsulta()
            .ToListAsync();
    }

    public async Task<Entities.Libro?> ObtenerPorId(int id)
    {
        return await _repositoryFactory.ObtenerRepository<Entities.Libro>()
            .ObtenerPorConsulta()
            .Include(l => l.IdEditorialNavigation)
            .Include(l => l.IdAutores)
            .Include(l => l.IdCategoria)
            .Include(l => l.Ejemplares)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<List<Entities.Libro>> ObtenerEjemplaresPorTitulo(string titulo)
    {
        return await _repositoryFactory.ObtenerRepository<Entities.Libro>().ObtenerPorConsulta()
            .Where(l => l.Titulo.Contains(titulo))
            .Select(l => new Entities.Libro
            {
                Id = l.Id,
                Titulo = l.Titulo,
                Ejemplares = l.Ejemplares
                    .Where(e => e.Disponible == true)
                    .ToList()
            })
            .ToListAsync();
    }


    public async Task<Entities.Libro> Crear(Entities.Libro libro)
    {
        libro.Activo = 1;
        libro.CreadoPor = ObtenerIdSesion();
        libro.FechaCreacion = DateTime.Now;
 
        await _repositoryFactory.ObtenerRepository<Entities.Libro>().Agregar(libro);
        return libro;
    }

    public async Task<Entities.Libro?> Actualizar(Entities.Libro libro)
    {
        var repo = _repositoryFactory.ObtenerRepository<Entities.Libro>();
        var existing = await repo.ObtenerPorId(libro.Id);

        if (existing == null)
            throw new Exception("Libro not found");

        libro.FechaCreacion = existing.FechaCreacion;
        libro.CreadoPor = existing.CreadoPor;
        libro.Activo = existing.Activo;
        libro.UltimaActualizacion = DateTime.Now;
        await repo.Actualizar(libro);
        return libro;
    }

    public async Task<bool> Eliminar(int id)
    {
        Entities.Libro libro =  await _repositoryFactory.ObtenerRepository<Entities.Libro>().ObtenerPorId(id);
        if (libro != null)
        {
            libro.Activo = 0;
            await _repositoryFactory.ObtenerRepository<Entities.Libro>().Actualizar(libro);
            return true;
        }
        return false;
    }
}