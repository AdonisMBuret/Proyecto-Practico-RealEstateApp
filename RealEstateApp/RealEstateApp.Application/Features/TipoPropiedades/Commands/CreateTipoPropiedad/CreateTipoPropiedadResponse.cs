namespace RealEstateApp.Application.Features.TipoPropiedades.Commands.CreateTipoPropiedad;

public class CreateTipoPropiedadResponse
{
    public int Id { get; set; }
    public bool Success { get; set; } = true;
    public string Mensaje { get; set; } = "Tipo de propiedad creado exitosamente";
}
