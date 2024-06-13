using minimalApi.DTOS;
using minimalApi.Identidades;

namespace minimalApi.Repositorio
{
    public interface IRepositorioPeliculas
    {
        Task ActualizarPelicula(Pelicula pelicula);
        Task AsignarActores(int idpelicula, List<ActorPelicula> actores);
        Task AsignarGeneros(int idpelicula, List<int> generosIds);
        Task BorrarPelicula(int idpelicula);
        Task<int> CrearPelicula(Pelicula pelicula);
        Task<bool> Existe(int idpelicula);
        Task<List<Pelicula>> ObtenerPelicula(PaginacionDTO paginacion);
        Task<Pelicula?> ObtenerPeliculaPorId(int idpeliculas);
        Task<List<Pelicula>> Filtrar(PeliculaFiltro peliculaFiltrarDTO);
    }
}