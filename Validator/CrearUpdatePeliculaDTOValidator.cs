using FluentValidation;
using minimalApi.DTOS;

namespace minimalApi.Validator
{
    public class CrearUpdatePeliculaDTOValidator:AbstractValidator<CreateUpdatePeliculaDTO>
    {
        public CrearUpdatePeliculaDTOValidator()
        {
            RuleFor(x => x.titulo).NotEmpty().WithMessage(Utilidades.CampoRequeridoMensaje)
                .MaximumLength(150).WithMessage(Utilidades.MaximnLengthMensaje);
        }
    }
}
