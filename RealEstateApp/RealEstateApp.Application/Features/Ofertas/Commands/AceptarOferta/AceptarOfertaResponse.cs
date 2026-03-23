namespace RealEstateApp.Application.Features.Ofertas.Commands.AceptarOferta;

public class AceptarOfertaResponse
{
    public int Id { get; set; }
    public string Mensaje { get; set; } = "Oferta aceptada exitosamente";
    public bool Success { get; set; } = true;
}
