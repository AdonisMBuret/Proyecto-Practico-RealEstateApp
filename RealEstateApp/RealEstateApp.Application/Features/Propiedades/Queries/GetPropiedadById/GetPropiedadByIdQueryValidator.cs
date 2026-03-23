using FluentValidation;

namespace RealEstateApp.Application.Features.Propiedades.Queries.GetPropiedadById;


public class GetPropiedadByIdQueryValidator : AbstractValidator<GetPropiedadByIdQuery>
{
    public GetPropiedadByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("El ID de la propiedad debe ser mayor a 0");
    }
}