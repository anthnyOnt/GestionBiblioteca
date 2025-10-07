// using GestionBiblioteca.Pages.Prestamo;

namespace GestionBiblioteca.Services.Prestamo;

public interface IPrestamoService
{
    Task<List<Entities.Prestamo>> ObtenerTodos();
    Task<Entities.Prestamo?> ObtenerPorId(int id);
    Task Crear(Entities.Prestamo prestamo, List<int> ejemplarIds);

    // Task Crear(PrestamoCreateViewModel model);
    
    // Task<Entities.Prestamo> PrestarEjemplares(List<Entities.PrestamoEjemplar> ejemplaresPrestados);
    // Task<Entities.PrestamoEjemplar> DevolverEjemplar();
}