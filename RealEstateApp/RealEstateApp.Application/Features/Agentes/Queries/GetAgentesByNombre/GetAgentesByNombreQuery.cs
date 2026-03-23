using MediatR;
using RealEstateApp.Application.ViewModels.Agentes;

namespace RealEstateApp.Application.Features.Agentes.Queries.GetAgentesByNombre;


public class GetAgentesByNombreQuery : IRequest<List<AgenteViewModel>>
{
    public string Nombre { get; set; } = null!;

    public GetAgentesByNombreQuery(string nombre)
    {
        Nombre = nombre;
    }
}
