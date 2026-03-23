using MediatR;

namespace RealEstateApp.Application.Features.Favoritos.Commands.AddPropiedadFavorita;

public class AddPropiedadFavoritaCommand : IRequest<AddPropiedadFavoritaResponse>
{
    public string ClienteId { get; set; } = null!;
    public int PropiedadId { get; set; }
}
