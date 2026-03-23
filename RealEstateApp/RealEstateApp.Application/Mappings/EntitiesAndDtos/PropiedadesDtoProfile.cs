using AutoMapper;
using RealEstateApp.Application.DTOs.Propiedades;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Application.Mappings.EntitiesAndDtos;


public class PropiedadesDtoProfile : Profile
{
    public PropiedadesDtoProfile()
    {
        CreateMap<Propiedad, PropiedadDTO>()
            .ForMember(dest => dest.TipoPropiedad, opt => opt.MapFrom(src => 
                src.TipoPropiedad != null ? src.TipoPropiedad.Nombre : "N/A"))
            .ForMember(dest => dest.TipoVenta, opt => opt.MapFrom(src => 
                src.TipoVenta != null ? src.TipoVenta.Nombre : "N/A"))
            .ForMember(dest => dest.NombreAgente, opt => opt.MapFrom(src => "N/A"))
            .ForMember(dest => dest.IdAgente, opt => opt.MapFrom(src => src.AgenteId))
            .ForMember(dest => dest.Mejoras, opt => opt.MapFrom(src => 
                src.PropiedadesMejoras.Select(pm => pm.Mejora.Nombre).ToList()))
            .ForMember(dest => dest.EstadoPropiedad, opt => opt.MapFrom(src => 
                src.Estado.ToString()));
            
        CreateMap<CreatePropiedadDTO, Propiedad>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Codigo, opt => opt.Ignore())
            .ForMember(dest => dest.Estado, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.TipoPropiedad, opt => opt.Ignore())
            .ForMember(dest => dest.TipoVenta, opt => opt.Ignore())
            .ForMember(dest => dest.Imagenes, opt => opt.Ignore())
            .ForMember(dest => dest.PropiedadesMejoras, opt => opt.Ignore())
            .ForMember(dest => dest.Ofertas, opt => opt.Ignore())
            .ForMember(dest => dest.Chats, opt => opt.Ignore())
            .ForMember(dest => dest.PropiedadesFavoritas, opt => opt.Ignore());
    }
}