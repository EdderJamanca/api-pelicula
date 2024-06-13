using Microsoft.AspNetCore.Identity;

namespace minimalApi.Identidades
{
    public class Comentario
    {
        public int idcomentario {  get; set; }
        public string cuerpo { get; set; } = null!;
        public int idpelicula { get; set; }

        public string idusuario { get; set; }=null!;

        public IdentityUser Usuario { get; set; } = null!;
    }
}
