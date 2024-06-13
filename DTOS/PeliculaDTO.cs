namespace minimalApi.DTOS
{
    public class PeliculaDTO
    {
        public int idpelicula { set; get; }
        public string titulo { set; get; } = null!;
        public bool EnCines { set; get; }
        public DateTime fechaLanzamiento { set; get; }
        public string? poster { set; get; }
        public List<ComentariosDTO> comentarios { set; get; }=new List<ComentariosDTO>();
        public List<GeneroDTO> generos { set; get; }=new List<GeneroDTO> { };
        public List<ActoresPeliculaDTO> actores { set; get; } = new List<ActoresPeliculaDTO>();
    }
}
