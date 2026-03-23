using AutoMapper;
using RealEstateApp.Application.DTOs.Propiedades;
using RealEstateApp.Application.ViewModels.Propiedades;

namespace RealEstateApp.Application.Mappings.ViewModelsToDto;

public class PropiedadesViewModelToDtoProfile : Profile
{
    public PropiedadesViewModelToDtoProfile()
    {
        CreateMap<SavePropiedadViewModel, CreatePropiedadDTO>();
        CreateMap<PropiedadViewModel, PropiedadDTO>();
        
       
        CreateMap<PropiedadDTO, PropiedadViewModel>();
        CreateMap<CreatePropiedadDTO, SavePropiedadViewModel>();
    }
}