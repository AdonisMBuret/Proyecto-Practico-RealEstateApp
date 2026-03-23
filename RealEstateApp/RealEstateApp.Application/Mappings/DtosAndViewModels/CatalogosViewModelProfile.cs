using AutoMapper;
using RealEstateApp.Application.ViewModels.Catalogos;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Application.Mappings.DtosAndViewModels;

public class CatalogosViewModelProfile : Profile
{
    public CatalogosViewModelProfile()
    {
        CreateMap<TipoPropiedad, TipoPropiedadViewModel>()
            .ForMember(dest => dest.CantidadPropiedades, opt => opt.Ignore()); 
        
        CreateMap<ViewModels.Catalogos.SaveTipoPropiedadViewModel, TipoPropiedad>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Propiedades, opt => opt.Ignore());

        CreateMap<TipoVenta, TipoVentaViewModel>()
            .ForMember(dest => dest.CantidadPropiedades, opt => opt.Ignore());
        
        CreateMap<ViewModels.Catalogos.SaveTipoVentaViewModel, TipoVenta>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Propiedades, opt => opt.Ignore());

        CreateMap<Mejora, MejoraViewModel>()
            .ForMember(dest => dest.CantidadPropiedades, opt => opt.Ignore());
        
        CreateMap<ViewModels.Catalogos.SaveMejoraViewModel, Mejora>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PropiedadesMejoras, opt => opt.Ignore());
    }
}
