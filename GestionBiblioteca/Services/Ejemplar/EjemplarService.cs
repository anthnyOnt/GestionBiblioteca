using GestionBiblioteca.Repository;
using Microsoft.EntityFrameworkCore;

namespace GestionBiblioteca.Services.Ejemplar;

public class EjemplarService: IEjemplarService
{
    private readonly IRepositoryFactory _repositoryFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public EjemplarService(IRepositoryFactory repositoryFactory, IHttpContextAccessor httpContextAccessor)
    {
        _repositoryFactory = repositoryFactory;
        _httpContextAccessor = httpContextAccessor;
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
    
    public async Task<List<Entities.Ejemplar>> ObtenerDisponibles()
    {
        return await _repositoryFactory.ObtenerRepository<Entities.Ejemplar>().ObtenerPorConsulta()
            .Where(e => e.Activo.Equals(1) && e.Disponible == true)
            .Include(e => e.IdLibroNavigation)
            .ToListAsync();
    }

    public async Task<List<Entities.Ejemplar>> ObtenerSeleccionados(List<int> seleccionados)
    {
        return await _repositoryFactory.ObtenerRepository<Entities.Ejemplar>().ObtenerPorConsulta()
            .Include(e => e.IdLibroNavigation)
            .Where(e => seleccionados.Contains(e.Id))
            .ToListAsync();
    }

    public async Task<Entities.Ejemplar> Crear(Entities.Ejemplar ejemplar)
    {
        ejemplar.FechaCreacion = DateTime.Now;
        ejemplar.CreadoPor = ObtenerIdSesion();
        ejemplar.Activo = 1;
        await _repositoryFactory.ObtenerRepository<Entities.Ejemplar>().Agregar(ejemplar);
        return ejemplar;
    }

    public async Task<Entities.Ejemplar> Actualizar(Entities.Ejemplar ejemplar)
    {
        var repo = _repositoryFactory.ObtenerRepository<Entities.Ejemplar>();
        var existing = await repo.ObtenerPorId(ejemplar.Id);
        
        if (existing == null)
            throw new Exception("Ejemplar no encontrado");
        ejemplar.FechaCreacion = existing.FechaCreacion;
        ejemplar.CreadoPor = existing.CreadoPor;
        ejemplar.Activo = existing.Activo;
        ejemplar.UltimaActualizacion = DateTime.Now;
        
        await repo.Actualizar(ejemplar);
        return ejemplar;
    }

    public async Task<bool> Eliminar(int id)
    {
        Entities.Ejemplar ejemplar = await _repositoryFactory.ObtenerRepository<Entities.Ejemplar>().ObtenerPorId(id);

        if (ejemplar != null)
        {
            ejemplar.Activo = 0;
            await _repositoryFactory.ObtenerRepository<Entities.Ejemplar>().Actualizar(ejemplar);
            return true;
        }

        return false;

    }
}