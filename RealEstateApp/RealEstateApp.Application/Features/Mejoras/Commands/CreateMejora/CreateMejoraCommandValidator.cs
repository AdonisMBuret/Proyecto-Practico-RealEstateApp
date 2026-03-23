using FluentValidation;

namespace RealEstateApp.Application.Features.Mejoras.Commands.CreateMejora
{
    public class CreateMejoraCommandValidator : AbstractValidator<CreateMejoraCommand>
    {
        public CreateMejoraCommandValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio")
                .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");

            RuleFor(x => x.Descripcion)
                .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres");
        }
    }
}
