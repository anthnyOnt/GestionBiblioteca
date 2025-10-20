using GestionBiblioteca.Context;
using GestionBiblioteca.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestionBiblioteca.Repository;

public class Repository<T>: IRepository<T> where T: class
{
    private readonly MyDbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(MyDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<List<T>> ObtenerTodos()
    {
        return await _dbSet.ToListAsync();
    }

    public IQueryable<T> ObtenerPorConsulta()
    {
        return _dbSet.AsQueryable();
    }
    
    

    public async Task<T?> ObtenerPorId(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task Agregar(T entidad)
    {
        _dbSet.Add(entidad);
        await _context.SaveChangesAsync();
    }
    
    public async Task Actualizar(T entidad)
    {
        var keyName = _context.Model.FindEntityType(typeof(T))!
            .FindPrimaryKey()!.Properties
            .Select(x => x.Name).Single();

        var id = typeof(T).GetProperty(keyName)!.GetValue(entidad);

        var existing = await _dbSet.FindAsync(id);
        if (existing == null)
            throw new Exception("Entity not found");

        _context.Entry(existing).CurrentValues.SetValues(entidad);

        await _context.SaveChangesAsync();
    }

    
    public async Task Eliminar(T entidad)
    {
        _dbSet.Remove(entidad);
        await _context.SaveChangesAsync();
    }
}