using FluentValidation;

namespace RealEstateApp.Application.Features.Mejoras.Commands.DeleteMejora
{
    public class DeleteMejoraCommandValidator : AbstractValidator<DeleteMejoraCommand>
    {
        public DeleteMejoraCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }
}