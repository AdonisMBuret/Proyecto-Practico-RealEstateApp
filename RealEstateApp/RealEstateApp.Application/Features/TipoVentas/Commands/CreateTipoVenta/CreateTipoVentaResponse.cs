namespace RealEstateApp.Application.Features.TipoVentas.Commands.CreateTipoVenta;

public class CreateTipoVentaResponse
{
    public int Id { get; set; }
    public bool Success { get; set; } = true;
    public string Mensaje { get; set; } = "Tipo de venta creado exitosamente";
}
