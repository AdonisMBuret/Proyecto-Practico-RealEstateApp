using MediatR;
using RealEstateApp.Application.ViewModels.Propiedades;

namespace RealEstateApp.Application.Features.Favoritos.Queries.GetPropiedadesFavoritas;

public class GetPropiedadesFavoritasQuery : IRequest<List<PropiedadViewModel>>
{
    public string ClienteId { get; set; } = null!;
}
