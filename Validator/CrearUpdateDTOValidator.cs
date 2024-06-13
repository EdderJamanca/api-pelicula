using FluentValidation;
using minimalApi.DTOS;
using minimalApi.Repositorio;

namespace minimalApi.Validator
{
    public class CrearUpdateDTOValidator:AbstractValidator<CrearUpdateDto>
    {
        public CrearUpdateDTOValidator(IRepositorioGeneros repositorio,IHttpContextAccessor httpContextAccessor)
        {
            var valorDeRutaId = httpContextAccessor.HttpContext?.Request.RouteValues["id"];
            var id = 0;
            if(valorDeRutaId is string valorString)
            {
                int.TryParse(valorString, out id );
            }

            RuleFor(x => x.Nombre).NotEmpty().WithMessage(Utilidades.CampoRequeridoMensaje)
                .MaximumLength(50).WithMessage(Utilidades.MaximnLengthMensaje)
                .Must(Utilidades.PrimeraLetraEndMayuscula).WithMessage(Utilidades.PrimeraLetraMayusculaMensaje)
                .MustAsync(async (nombre, _) => {
                    var existe = await repositorio.UnicoGenero(id:0, nombre);
                    return !existe;
                }).WithMessage(g=>$"Ya existe un género con el nombre {g.Nombre}");
        }


    }
}
