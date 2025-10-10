using GestionBiblioteca.Entities;
using GestionBiblioteca.Repository;
using Microsoft.EntityFrameworkCore;

namespace GestionBiblioteca.Services.Prestamo;

public class PrestamoService: IPrestamoService
{
    private readonly IRepositoryFactory _repositoryFactory;

    public PrestamoService(IRepositoryFactory repositoryFactory)
    {
        _repositoryFactory = repositoryFactory;
    }
    
    public async Task<List<Entities.Prestamo>> ObtenerTodos()
    {
        return await _repositoryFactory.ObtenerRepository<Entities.Prestamo>().ObtenerPorConsulta()
            .Include(p => p.IdUsuarioNavigation)
            .Include(p => p.PrestamoEjemplares)
            .Where(p => p.Activo == 1)
            .ToListAsync();
    }
    
    public async Task<Entities.Prestamo?> ObtenerPorId(int id)
    {
        return await _repositoryFactory.ObtenerRepository<Entities.Prestamo>().ObtenerPorConsulta()
            .Include(p => p.IdUsuarioNavigation)
            .Include(p => p.PrestamoEjemplares)
            .ThenInclude(pe => pe.IdEjemplarNavigation)
            .ThenInclude(l => l.IdLibroNavigation)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task Crear(int idUsuario, List<PrestamoEjemplar> ejemplares)
        {
            if (idUsuario <= 0) throw new ArgumentOutOfRangeException(nameof(idUsuario));
            if (ejemplares == null || ejemplares.Count == 0) throw new ArgumentException("Debe incluir al menos un ejemplar.", nameof(ejemplares));

            var idsUnicos = new HashSet<int>();
            foreach (var l in ejemplares)
            {
                if (l.IdEjemplar <= 0) throw new InvalidOperationException("IdEjemplar inválido.");
                if (!idsUnicos.Add(l.IdEjemplar)) throw new InvalidOperationException($"Ejemplar repetido: {l.IdEjemplar}.");
                if (l.FechaLimite == null || l.FechaLimite.Value.Date < DateTime.Today)
                    throw new InvalidOperationException($"Fecha límite inválida para ejemplar {l.IdEjemplar}.");
            }

            using var tx = await _repositoryFactory.BeginTransaction();
            try
            {
                var nuevo = new Entities.Prestamo
                {
                    IdUsuario = idUsuario,
                    FechaPrestamo = DateTime.Now,
                    Activo = 1,
                    Cancelado = false
                };
                await _repositoryFactory.ObtenerRepository<Entities.Prestamo>().Agregar(nuevo);

                var ejemplarRepo = _repositoryFactory.ObtenerRepository<Entities.Ejemplar>();
                foreach (var l in ejemplares)
                {
                    var ej = await ejemplarRepo.ObtenerPorId(l.IdEjemplar)
                             ?? throw new InvalidOperationException($"Ejemplar {l.IdEjemplar} no existe.");
                    if (ej.Disponible != true)
                        throw new InvalidOperationException($"Ejemplar {l.IdEjemplar} no disponible.");
                    ej.Disponible = false;
                    await ejemplarRepo.Actualizar(ej);
                }

                var peRepo = _repositoryFactory.ObtenerRepository<PrestamoEjemplar>();
                foreach (var l in ejemplares)
                {
                    var pe = new PrestamoEjemplar
                    {
                        IdPrestamo = nuevo.Id,
                        IdEjemplar = l.IdEjemplar,
                        FechaLimite = l.FechaLimite,
                        Activo = 1
                    };
                    await peRepo.Agregar(pe);
                }

                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }


    public async Task<Entities.Prestamo> PrestarEjemplares(Entities.Prestamo prestamo)
    {
        throw new NotImplementedException();
    }
    
}