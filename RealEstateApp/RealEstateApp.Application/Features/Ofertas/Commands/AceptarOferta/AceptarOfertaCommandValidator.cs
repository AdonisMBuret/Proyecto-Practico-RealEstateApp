using FluentValidation;

namespace RealEstateApp.Application.Features.Ofertas.Commands.AceptarOferta;

public class AceptarOfertaCommandValidator : AbstractValidator<AceptarOfertaCommand>
{
    public AceptarOfertaCommandValidator()
    {
        RuleFor(x => x.OfertaId)
            .GreaterThan(0).WithMessage("El ID de la oferta debe ser mayor a 0");

        RuleFor(x => x.AgenteId)
            .NotEmpty().WithMessage("El ID del agente es requerido");
    }
}
