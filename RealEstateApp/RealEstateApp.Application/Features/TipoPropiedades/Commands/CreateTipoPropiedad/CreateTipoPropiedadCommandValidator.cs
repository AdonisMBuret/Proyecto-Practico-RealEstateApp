using FluentValidation;

namespace RealEstateApp.Application.Features.TipoPropiedades.Commands.CreateTipoPropiedad
{
    public class CreateTipoPropiedadCommandValidator : AbstractValidator<CreateTipoPropiedadCommand>
    {
        public CreateTipoPropiedadCommandValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio")
                .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");

            RuleFor(x => x.Descripcion)
                .NotEmpty().WithMessage("La descripción es requerida") 
                .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres");
        }
    }
}