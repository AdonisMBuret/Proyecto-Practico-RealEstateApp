using FluentValidation;

namespace RealEstateApp.Application.Features.Propiedades.Queries.GetAllPropiedades;


public class GetAllPropiedadesQueryValidator : AbstractValidator<GetAllPropiedadesQuery>
{
    public GetAllPropiedadesQueryValidator()
    {
        
        RuleFor(x => x.SoloDisponibles)
            .NotNull()
            .WithMessage("El parámetro SoloDisponibles no puede ser nulo");
    }
}