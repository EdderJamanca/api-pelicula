using minimalApi.DTOS;
using FluentValidation;
namespace minimalApi.Validator
{
    public class EditarClaimDTOValidator:AbstractValidator<EditarClaimDTO>
    {
       public EditarClaimDTOValidator() 
       {
            RuleFor(x => x.Email).NotEmpty().WithMessage(Utilidades.CampoRequeridoMensaje)
                .MaximumLength(256).WithMessage(Utilidades.MaximnLengthMensaje)
                .EmailAddress().WithMessage(Utilidades.EmailMensaje);
        }
    }
}
