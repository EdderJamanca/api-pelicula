using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace minimalApi.Identidades
{
    public class Pelicula
    {

        public int idpelicula { set; get; }
        public string titulo { set; get; } = null!; 
        public bool EnCines { set; get; }   
        public DateTime fechaLanzamiento {  set; get; }
        public string? poster {  set; get; }

        public List<Comentario> comentarios { set; get; }=new List<Comentario>();
        public List<GeneroPelicula> GeneroPeliculas { set; get; }= new List<GeneroPelicula>();
        public List<ActorPelicula> ActorPelicula {  set; get; }=new List<ActorPelicula>();

    }
}
