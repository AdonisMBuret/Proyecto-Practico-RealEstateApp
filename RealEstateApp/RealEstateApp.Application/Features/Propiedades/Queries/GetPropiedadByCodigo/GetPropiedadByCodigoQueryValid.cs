using FluentValidation;

namespace RealEstateApp.Application.Features.Propiedades.Queries.GetPropiedadByCodigo
{
    
    public class GetPropiedadByCodigoQueryValidator : AbstractValidator<GetPropiedadByCodigoQuery>
    {
        public GetPropiedadByCodigoQueryValidator()
        {
            RuleFor(x => x.Codigo)
                .NotEmpty()
                .WithMessage("El código de la propiedad es requerido.")
                .NotNull()
                .WithMessage("El código de la propiedad no puede ser nulo.")
                .MinimumLength(3)
                .WithMessage("El código de la propiedad debe tener al menos 3 caracteres.")
                .MaximumLength(20)
                .WithMessage("El código de la propiedad no puede exceder 20 caracteres.")
                .Matches(@"^[a-zA-Z0-9\-_]+$")
                .WithMessage("El código de la propiedad solo puede contener letras, números, guiones y guiones bajos.");
        }
    }
}