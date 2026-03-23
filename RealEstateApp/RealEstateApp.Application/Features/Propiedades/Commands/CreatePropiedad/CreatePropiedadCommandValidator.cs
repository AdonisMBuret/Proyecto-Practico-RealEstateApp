using FluentValidation;

namespace RealEstateApp.Application.Features.Propiedades.Commands.CreatePropiedad;


public class CreatePropiedadCommandValidator : AbstractValidator<CreatePropiedadCommand>
{
    public CreatePropiedadCommandValidator()
    {
        RuleFor(x => x.AgenteId)
            .NotEmpty().WithMessage("El ID del agente es requerido")
            .NotNull().WithMessage("El ID del agente no puede ser nulo");

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

        RuleFor(x => x.UrlImagenes)
            .NotEmpty().WithMessage("Debe agregar al menos una imagen")
            .Must(x => x.Count >= 1 && x.Count <= 4)
            .WithMessage("Debe agregar entre 1 y 4 imágenes");
    }
}
