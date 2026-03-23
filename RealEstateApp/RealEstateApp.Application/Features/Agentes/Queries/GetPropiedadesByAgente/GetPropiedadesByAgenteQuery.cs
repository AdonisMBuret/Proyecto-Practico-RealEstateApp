using MediatR;
using RealEstateApp.Application.ViewModels.Propiedades;

namespace RealEstateApp.Application.Features.Agentes.Queries.GetPropiedadesByAgente;


public class GetPropiedadesByAgenteQuery : IRequest<List<PropiedadViewModel>>
{
    public string AgenteId { get; set; } = null!;
    public bool SoloDisponibles { get; set; } = true;

    public GetPropiedadesByAgenteQuery(string agenteId, bool soloDisponibles = true)
    {
        AgenteId = agenteId;
        SoloDisponibles = soloDisponibles;
    }
}
