using FluentValidation;

namespace RealEstateApp.Application.Features.Propiedades.Queries.GetPropertyById
{
    public class GetPropertyByIdQueryValidator : AbstractValidator<GetPropertyByIdQuery>
    {
        public GetPropertyByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("El Id de la propiedad debe ser mayor a 0");
        }
    }
}