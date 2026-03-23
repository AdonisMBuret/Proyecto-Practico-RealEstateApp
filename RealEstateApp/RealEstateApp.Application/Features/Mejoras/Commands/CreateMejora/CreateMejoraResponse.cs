namespace RealEstateApp.Application.Features.Mejoras.Commands.CreateMejora;

public class CreateMejoraResponse
{
    public int Id { get; set; }
    public bool Success { get; set; } = true;
    public string Mensaje { get; set; } = "Mejora creada exitosamente";
}
