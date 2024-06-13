using AutoMapper;
using minimalApi.DTOS;
using minimalApi.Identidades;

namespace minimalApi.Utilidades
{
    public class AutoMapperProfile:Profile
    {
        public AutoMapperProfile() {
            CreateMap<CrearUpdateDto, Genero>();
            CreateMap<Genero, GeneroDTO>();

            CreateMap<CrearUpdateActores, Actores>()
                .ForMember(x => x.foto, opciones => opciones.Ignore());
            CreateMap<Actores, ActorDTO>();

            CreateMap<CreateUpdatePeliculaDTO, Pelicula>().ForMember(x => x.poster, option => option.Ignore());
            CreateMap<Pelicula, PeliculaDTO>()
                .ForMember(x => x.generos,
                entidad => entidad.MapFrom(p => p.GeneroPeliculas
                .Select(gp => new GeneroDTO { Id = gp.idgenero, Nombre = gp.Genero.Nombre })))
                .ForMember(x => x.actores, entidad => entidad.MapFrom(p => p.ActorPelicula
                .Select(ap => new ActoresPeliculaDTO { idactores = ap.idactor, nombre = ap.actores.nombre, personajes = ap.personajes })));

            CreateMap<CreateUpdateComentario, Comentario>();
            CreateMap<Comentario,ComentariosDTO>();

            CreateMap<AsignarActorPeliculaDTO, ActorPelicula>();
        }
    }
}
