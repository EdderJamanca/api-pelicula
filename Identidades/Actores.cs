namespace minimalApi.Identidades
{
    public class Actores
    {

        public int idactores { set; get; }
        public string nombre { set; get; } = null!;
        public DateTime fechaNacimiento { set; get; }
        // ? -> esto significa que va aceptar valores vacios
        public string? foto { set; get; }
    }
}
