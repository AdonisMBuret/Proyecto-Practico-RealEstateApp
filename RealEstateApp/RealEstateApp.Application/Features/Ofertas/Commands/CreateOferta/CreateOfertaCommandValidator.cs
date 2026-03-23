using FluentValidation;

namespace RealEstateApp.Application.Features.Ofertas.Commands.CreateOferta;

public class CreateOfertaCommandValidator : AbstractValidator<CreateOfertaCommand>
{
    public CreateOfertaCommandValidator()
    {
        RuleFor(x => x.PropiedadId)
            .GreaterThan(0).WithMessage("El ID de la propiedad debe ser mayor a 0");

        RuleFor(x => x.ClienteId)
            .NotEmpty().WithMessage("El ID del cliente es requerido");

        RuleFor(x => x.Monto)
            .GreaterThan(0).WithMessage("El monto de la oferta debe ser mayor a 0")
            .LessThanOrEqualTo(100000000).WithMessage("El monto de la oferta no puede exceder RD$100,000,000");
    }
}
