using AutoMapper;
using RealEstateApp.Application.DTOs.Propiedades;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Enums;

namespace RealEstateApp.Application.Mappings
{
    public class PropiedadProfile : Profile
    {
        public PropiedadProfile()
        {
            CreateMap<Propiedad, PropiedadDTO>()
                .ForMember(dest => dest.TipoPropiedad, opt => opt.MapFrom(src => src.TipoPropiedad.Nombre))
                .ForMember(dest => dest.TipoVenta, opt => opt.MapFrom(src => src.TipoVenta.Nombre))
                .ForMember(dest => dest.NombreAgente, opt => opt.Ignore())
                .ForMember(dest => dest.Mejoras, opt => opt.MapFrom(src => src.PropiedadesMejoras.Select(pm => pm.Mejora.Nombre).ToList()))
                .ForMember(dest => dest.EstadoPropiedad, opt => opt.MapFrom(src => src.Estado == EstadoPropiedad.Disponible ? "Disponible" : "Vendida"))
                .ForMember(dest => dest.TamanoMetros, opt => opt.MapFrom(src => (decimal)src.TamanoEnMetros));
        }
    }
}