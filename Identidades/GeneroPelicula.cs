namespace minimalApi.Identidades
{
    public class GeneroPelicula
    {
        public int idpelicula {  get; set; }
        public int idgenero { get; set; }

        public Genero Genero { get; set; } = null!;
        public Pelicula pelicula { get; set; }=null!;
    }
}
