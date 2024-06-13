namespace minimalApi.Identidades
{
    public class ActorPelicula
    {

        public  int idpelicula {  get; set; }
        public int idactor {  get; set; }
        public Actores actores { get; set; } = null!;
        public Pelicula pelicula { get; set; } = null!;
        public int orden {  get; set; }
        public string personajes { get; set; }=null!;
    }
}
