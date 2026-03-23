using MediatR;
using RealEstateApp.Application.ViewModels.Propiedades;

namespace RealEstateApp.Application.Features.Propiedades.Queries.GetPropiedadByCodigo;


public class GetPropiedadByCodigoQuery : IRequest<PropiedadViewModel?>
{
    public string Codigo { get; set; } = null!;

    public GetPropiedadByCodigoQuery(string codigo)
    {
        Codigo = codigo;
    }
}
