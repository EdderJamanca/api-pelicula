using FluentValidation;
using minimalApi.DTOS;

namespace minimalApi.Validator
{
    public class CrearActorDTOValidator:AbstractValidator<CrearUpdateActores>
    {
        public CrearActorDTOValidator() 
        {
            RuleFor(x => x.nombre).NotEmpty().WithMessage(Utilidades.CampoRequeridoMensaje)
                .MaximumLength(150).WithMessage(Utilidades.MaximnLengthMensaje);

            var fechaMinima = new DateTime(1900, 1, 1);

            RuleFor(x => x.fechaNacimiento).GreaterThanOrEqualTo(fechaMinima)
            .WithMessage(Utilidades.GreateThanOrEqualToMensaje(fechaMinima));
        }
    }
}
