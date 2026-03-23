namespace RealEstateApp.Application.Features.Ofertas.Commands.RechazarOferta;

public class RechazarOfertaResponse
{
    public int Id { get; set; }
    public string Mensaje { get; set; } = "Oferta rechazada";
    public bool Success { get; set; } = true;
}
