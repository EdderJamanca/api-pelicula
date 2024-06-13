using minimalApi.DTOS;
using minimalApi.Identidades;

namespace minimalApi.Repositorio
{
    public interface IRepositorioActores
    {
        Task ActualizarActores(Actores actores);
        Task BorrarActores(int idactores);
        Task<List<Actores>> ObtenerActores(PaginacionDTO paginacion);
        Task<int> CrearActor(Actores actores);
        Task<bool> ExisteActor(int idactores);
        Task<Actores?> ObtenerActoresPorId(int id);
        Task<Actores?> ActoresFiltroPorNombre(string nombre);
        Task<List<int>> ExistenActores(List<int> idsactore);
    }
}