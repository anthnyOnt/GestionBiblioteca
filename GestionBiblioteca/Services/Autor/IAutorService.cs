namespace GestionBiblioteca.Services.Autor;

public interface IAutorService
{
    Task<List<Entities.Autor>> ObtenerTodos();
    Task<Entities.Autor?> ObtenerPorId(int id);
    Task<Entities.Autor> Crear(Entities.Autor autor);
    Task<Entities.Autor?> Actualizar(Entities.Autor autor);
    Task<bool> Eliminar(int id);
}