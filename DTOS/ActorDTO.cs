namespace minimalApi.DTOS
{
    public class ActorDTO
    {
        public int idactores {  get; set; }
        public string nombre { get; set; } = null!;
        public DateTime fechaNacimiento { get; set; }
        public string? foto { get; set; }


    }
}
