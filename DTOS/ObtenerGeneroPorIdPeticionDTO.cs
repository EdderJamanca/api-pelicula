using AutoMapper;
using minimalApi.Repositorio;

namespace minimalApi.DTOS
{
    public class ObtenerGeneroPorIdPeticionDTO
    {
        public IRepositorioGeneros Repositorio { get; set; }
        public int Id { get; set; }
        public IMapper Mapper { get; set; }
    }
}
