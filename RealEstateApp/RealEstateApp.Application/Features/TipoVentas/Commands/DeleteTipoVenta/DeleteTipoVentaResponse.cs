namespace RealEstateApp.Application.Features.TipoVentas.Commands.DeleteTipoVenta;

public class DeleteTipoVentaResponse
{
    public bool Success { get; set; } = true;
    public string Mensaje { get; set; } = "Tipo de venta y sus propiedades asociadas eliminadas exitosamente";
}
