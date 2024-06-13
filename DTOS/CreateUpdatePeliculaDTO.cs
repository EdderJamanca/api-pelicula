namespace minimalApi.DTOS
{
    public class CreateUpdatePeliculaDTO
    {
        //public int idpelicula { set; get; }
        public string titulo { set; get; } = null!;
        public bool EnCines { set; get; }
        public DateTime fechaLanzamiento { set; get; }
        public IFormFile? poster { set; get; }
    }
}
