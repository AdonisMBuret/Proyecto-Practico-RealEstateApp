using FluentValidation;

namespace RealEstateApp.Application.Features.Mejoras.Commands.UpdateMejora;

public class UpdateMejoraCommandValidator : AbstractValidator<UpdateMejoraCommand>
{
    public UpdateMejoraCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("El ID debe ser mayor que 0");

        When(x => !string.IsNullOrWhiteSpace(x.Nombre), () =>
        {
            RuleFor(x => x.Nombre)
                .MaximumLength(150)
                .WithMessage("El nombre no puede exceder 150 caracteres");
        });

        When(x => !string.IsNullOrWhiteSpace(x.Descripcion), () =>
        {
            RuleFor(x => x.Descripcion)
                .MaximumLength(500)
                .WithMessage("La descripción no puede exceder 500 caracteres");
        });

        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Nombre) || !string.IsNullOrWhiteSpace(x.Descripcion))
            .WithMessage("Debe proporcionar al menos un campo para actualizar (Nombre o Descripcion)")
            .WithName("Request");
    }
}
