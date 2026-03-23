namespace RealEstateApp.Application.Features.Ofertas.Commands.CreateOferta;

public class CreateOfertaResponse
{
    public int Id { get; set; }
    public string Mensaje { get; set; } = "Oferta enviada exitosamente";
    public bool Success { get; set; } = true;
}
