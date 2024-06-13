namespace minimalApi.DTOS
{
    public class ComentariosDTO
    {
        public int idcomentario { get; set; }
        public string cuerpo { get; set; } = null!;
        public int idpelicula { get; set; }
    }
}
