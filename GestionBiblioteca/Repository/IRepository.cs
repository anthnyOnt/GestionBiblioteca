using GestionBiblioteca.Context;
using Microsoft.EntityFrameworkCore;

namespace GestionBiblioteca.Repository;

public interface IRepository<T> where T : class
{
    public Task<List<T>> ObtenerTodos();
    public IQueryable<T> ObtenerPorConsulta();
    Task<T?> ObtenerPorId(int id);
    Task Agregar(T entidad);
    Task Actualizar(T entidad);
    Task Eliminar(T entidad);
    
}