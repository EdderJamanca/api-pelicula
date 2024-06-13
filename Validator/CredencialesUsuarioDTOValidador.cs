using FluentValidation;
using minimalApi.DTOS;
using System.Data;

namespace minimalApi.Validator
{
    public class CredencialesUsuarioDTOValidador:AbstractValidator<CredencialesUsuarioDTO>
    {
        public CredencialesUsuarioDTOValidador()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage(Utilidades.CampoRequeridoMensaje)
                .MaximumLength(256).WithMessage(Utilidades.MaximnLengthMensaje)
                .EmailAddress().WithMessage(Utilidades.EmailMensaje);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(Utilidades.CampoRequeridoMensaje);
        }

    
    }
}
