using FluentValidation;

namespace RealEstateApp.Application.Features.Propiedades.Commands.UpdatePropiedad;


public class UpdatePropiedadCommandValidator : AbstractValidator<UpdatePropiedadCommand>
{
    public UpdatePropiedadCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El ID de la propiedad debe ser válido");

        RuleFor(x => x.TipoPropiedadId)
            .GreaterThan(0).WithMessage("Debe seleccionar un tipo de propiedad válido");

        RuleFor(x => x.TipoVentaId)
            .GreaterThan(0).WithMessage("Debe seleccionar un tipo de venta válido");

        RuleFor(x => x.Precio)
            .GreaterThan(0).WithMessage("El precio debe ser mayor a 0")
            .LessThanOrEqualTo(999999999999).WithMessage("El precio excede el límite permitido");

        RuleFor(x => x.TamanoEnMetros)
            .GreaterThan(0).WithMessage("El tamaño en metros debe ser mayor a 0")
            .LessThanOrEqualTo(1000000).WithMessage("El tamaño excede el límite permitido");

        RuleFor(x => x.CantidadHabitaciones)
            .GreaterThanOrEqualTo(0).WithMessage("La cantidad de habitaciones no puede ser negativa")
            .LessThanOrEqualTo(50).WithMessage("La cantidad de habitaciones excede el límite permitido");

        RuleFor(x => x.CantidadBanos)
            .GreaterThanOrEqualTo(0).WithMessage("La cantidad de baños no puede ser negativa")
            .LessThanOrEqualTo(20).WithMessage("La cantidad de baños excede el límite permitido");

        RuleFor(x => x.Descripcion)
            .NotEmpty().WithMessage("La descripción es requerida")
            .MaximumLength(1000).WithMessage("La descripción no puede exceder los 1000 caracteres");
    }
}
