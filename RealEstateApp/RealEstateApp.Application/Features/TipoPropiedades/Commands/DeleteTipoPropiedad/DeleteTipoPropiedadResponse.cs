namespace RealEstateApp.Application.Features.TipoPropiedades.Commands.DeleteTipoPropiedad;

public class DeleteTipoPropiedadResponse
{
    public bool Success { get; set; } = true;
    public string Mensaje { get; set; } = "Tipo de propiedad y sus propiedades asociadas eliminadas exitosamente";
}
