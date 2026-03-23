using MediatR;
using RealEstateApp.Application.ViewModels.Propiedades;

namespace RealEstateApp.Application.Features.Propiedades.Queries.GetAllPropiedades;


public class GetAllPropiedadesQuery : IRequest<List<PropiedadViewModel>>
{
    public bool SoloDisponibles { get; set; } = true;
}
