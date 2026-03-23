using MediatR;
using RealEstateApp.Application.ViewModels.Propiedades;

namespace RealEstateApp.Application.Features.Propiedades.Queries.GetPropiedadById;


public class GetPropiedadByIdQuery : IRequest<PropiedadViewModel?>
{
    public int Id { get; set; }

    public GetPropiedadByIdQuery(int id)
    {
        Id = id;
    }
}
