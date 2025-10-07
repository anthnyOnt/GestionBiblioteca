namespace GestionBiblioteca.Services.Editorial;

public interface IEditorialService
{
    Task<List<Entities.Editorial>> ObtenerTodos();
    Task<Entities.Editorial?> ObtenerPorId(int id);
    Task<Entities.Editorial> Crear(Entities.Editorial editorial);
    Task<Entities.Editorial?> Actualizar(Entities.Editorial editorial);
    Task<bool> Eliminar(int id);
}