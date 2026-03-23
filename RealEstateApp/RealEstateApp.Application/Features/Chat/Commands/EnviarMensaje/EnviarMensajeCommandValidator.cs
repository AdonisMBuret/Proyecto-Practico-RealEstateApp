using FluentValidation;

namespace RealEstateApp.Application.Features.Chat.Commands.EnviarMensaje;


public class EnviarMensajeCommandValidator : AbstractValidator<EnviarMensajeCommand>
{
    public EnviarMensajeCommandValidator()
    {
        RuleFor(x => x.PropiedadId)
            .GreaterThan(0)
            .WithMessage("El ID de la propiedad debe ser mayor a 0");

        RuleFor(x => x.EmisorId)
            .NotEmpty()
            .WithMessage("El ID del emisor es requerido")
            .Length(1, 450)
            .WithMessage("El ID del emisor debe tener entre 1 y 450 caracteres");

        RuleFor(x => x.ReceptorId)
            .NotEmpty()
            .WithMessage("El ID del receptor es requerido")
            .Length(1, 450)
            .WithMessage("El ID del receptor debe tener entre 1 y 450 caracteres");

        RuleFor(x => x.Contenido)
            .NotEmpty()
            .WithMessage("El contenido del mensaje es requerido")
            .MinimumLength(1)
            .WithMessage("El mensaje debe tener al menos 1 carácter")
            .MaximumLength(1000)
            .WithMessage("El mensaje no puede exceder 1000 caracteres")
            .Matches(@"^[\s\S]*\S[\s\S]*$")
            .WithMessage("El mensaje no puede contener solo espacios en blanco");

        RuleFor(x => x.EmisorId)
            .NotEqual(x => x.ReceptorId)
            .WithMessage("El emisor y receptor no pueden ser el mismo usuario");
    }
}