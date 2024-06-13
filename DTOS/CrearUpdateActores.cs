namespace minimalApi.DTOS
{
    public class CrearUpdateActores
    {
        //public int idactores { get; set; }
        public string nombre { get; set; } = null!;
        public DateTime fechaNacimiento { get; set; }
        public IFormFile? foto { get; set; }

    }
}
