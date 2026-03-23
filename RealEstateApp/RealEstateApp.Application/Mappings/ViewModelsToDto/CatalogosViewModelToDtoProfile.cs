using AutoMapper;
using RealEstateApp.Application.DTOs.Catalogos;
using RealEstateApp.Application.ViewModels.Catalogos;

namespace RealEstateApp.Application.Mappings.ViewModelsToDto;


public class CatalogosViewModelToDtoProfile : Profile
{
    public CatalogosViewModelToDtoProfile()
    {
     
        CreateMap<SaveTipoPropiedadViewModel, SaveCatalogoDTO>();
        CreateMap<TipoPropiedadViewModel, CatalogoDTO>();
        
   
        CreateMap<SaveTipoVentaViewModel, SaveCatalogoDTO>();
        CreateMap<TipoVentaViewModel, CatalogoDTO>();
        

        CreateMap<SaveMejoraViewModel, SaveCatalogoDTO>();
        CreateMap<MejoraViewModel, CatalogoDTO>();
    }
}