using FluentValidation;

namespace RealEstateApp.Application.Features.TipoVentas.Commands.CreateTipoVenta;

public class CreateTipoVentaCommandValidator : AbstractValidator<CreateTipoVentaCommand>
{
    public CreateTipoVentaCommandValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres");

        RuleFor(x => x.Descripcion)
            .NotEmpty().WithMessage("La descripción es requerida")
            .MaximumLength(500).WithMessage("La descripción no puede exceder los 500 caracteres");
    }
}
