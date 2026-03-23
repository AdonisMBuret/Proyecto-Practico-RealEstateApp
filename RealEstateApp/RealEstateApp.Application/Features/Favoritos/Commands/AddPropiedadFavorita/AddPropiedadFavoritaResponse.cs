namespace RealEstateApp.Application.Features.Favoritos.Commands.AddPropiedadFavorita;

public class AddPropiedadFavoritaResponse
{
    public int Id { get; set; }
    public string Mensaje { get; set; } = "Propiedad agregada a favoritos exitosamente";
    public bool Success { get; set; } = true;
}
