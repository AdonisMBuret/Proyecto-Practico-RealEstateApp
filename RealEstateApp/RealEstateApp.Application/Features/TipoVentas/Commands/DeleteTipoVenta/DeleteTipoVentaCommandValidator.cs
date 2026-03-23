using FluentValidation;

namespace RealEstateApp.Application.Features.TipoVentas.Commands.DeleteTipoVenta
{
    public class DeleteTipoVentaCommandValidator : AbstractValidator<DeleteTipoVentaCommand>
    {
        public DeleteTipoVentaCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }
}