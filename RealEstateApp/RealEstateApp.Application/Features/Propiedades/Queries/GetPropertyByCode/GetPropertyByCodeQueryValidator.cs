using FluentValidation;

namespace RealEstateApp.Application.Features.Propiedades.Queries.GetPropertyByCode
{
    public class GetPropertyByCodeQueryValidator : AbstractValidator<GetPropertyByCodeQuery>
    {
        public GetPropertyByCodeQueryValidator()
        {
            RuleFor(x => x.Codigo)
                .NotEmpty()
                .WithMessage("El código de la propiedad es obligatorio")
                .Length(6)
                .WithMessage("El código debe tener exactamente 6 dígitos")
                .Matches(@"^\d{6}$")
                .WithMessage("El código debe contener solo números");
        }
    }
}