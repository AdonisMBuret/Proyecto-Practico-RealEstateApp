using FluentValidation;

namespace RealEstateApp.Application.Features.TipoPropiedades.Commands.DeleteTipoPropiedad
{
    public class DeleteTipoPropiedadCommandValidator : AbstractValidator<DeleteTipoPropiedadCommand>
    {
        public DeleteTipoPropiedadCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }
}