namespace RealEstateApp.Application.Features.Propiedades.Commands.CreatePropiedad;


public class CreatePropiedadResponse
{
    public int Id { get; set; }
    public string Codigo { get; set; } = null!;
    public string Mensaje { get; set; } = "Propiedad creada exitosamente";
    public bool Success { get; set; } = true;
}
