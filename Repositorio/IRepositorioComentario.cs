using minimalApi.DTOS;
using minimalApi.Identidades;

namespace minimalApi.Repositorio
{
    public interface IRepositorioComentario
    {
        Task actualizar(Comentario comentario);
        Task Borrar(int idcomentario,int idpelicula);
        Task<int> CrearComentario(Comentario comentario);
        Task<bool> Existe(int idcomentario,int idpelicula);
        Task<List<Comentario>> ObtenerComentario(PaginacionDTO paginacion,int idpelicula);
        Task<Comentario?> ObtenerComentarioPorId(int idcomentario, int idpelicula);
    }
}