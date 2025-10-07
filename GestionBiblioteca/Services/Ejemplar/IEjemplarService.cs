namespace GestionBiblioteca.Services.Ejemplar;

public interface IEjemplarService
{
    Task<List<Entities.Ejemplar>> ObtenerDisponibles();
    Task<List<Entities.Ejemplar>> ObtenerSeleccionados(List<int> seleccionados);
}