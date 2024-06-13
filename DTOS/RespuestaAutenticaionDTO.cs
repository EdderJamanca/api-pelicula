namespace minimalApi.DTOS
{
    public class RespuestaAutenticaionDTO
    {
        public string Token { get; set; } = null!;
        public DateTime Expiracion {  get; set; }

    }
}
