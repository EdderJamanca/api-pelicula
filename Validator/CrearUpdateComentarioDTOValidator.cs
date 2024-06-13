using FluentValidation;
using minimalApi.DTOS;

namespace minimalApi.Validator
{
    public class CrearUpdateComentarioDTOValidator:AbstractValidator<CreateUpdateComentario>
    {
        public CrearUpdateComentarioDTOValidator()
        {
            RuleFor(x=>x.cuerpo).NotEmpty().WithMessage(Utilidades.CampoRequeridoMensaje);
        }
    }
}
