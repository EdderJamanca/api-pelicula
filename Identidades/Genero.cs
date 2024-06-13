namespace minimalApi.Identidades
{
    public class Genero
    {

        public int Id { get; set; }
        //null!; se pone para perdonar el nulo
        public string Nombre { get; set; } = null!;
    }
}
