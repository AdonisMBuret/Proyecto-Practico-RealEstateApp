using AutoMapper;
using RealEstateApp.Application.DTOs.Catalogos;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Application.Mappings.EntitiesAndDtos;

public class CatalogosDtoProfile : Profile
{
    public CatalogosDtoProfile()
    {

        CreateMap<TipoPropiedad, CatalogoDTO>();
        
        CreateMap<SaveCatalogoDTO, TipoPropiedad>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Propiedades, opt => opt.Ignore());

        CreateMap<TipoVenta, CatalogoDTO>();
        
        CreateMap<SaveCatalogoDTO, TipoVenta>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Propiedades, opt => opt.Ignore());

        CreateMap<Mejora, CatalogoDTO>();
        
        CreateMap<SaveCatalogoDTO, Mejora>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PropiedadesMejoras, opt => opt.Ignore());
    }
}