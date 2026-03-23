using AutoMapper;
using RealEstateApp.Application.DTOs.Propiedades;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Enums;
using System.Linq;

namespace RealEstateApp.Application.Mappings
{
    public class PropiedadApiProfile : Profile
    {
        public PropiedadApiProfile()
        {
            CreateMap<Propiedad, PropiedadApiDTO>()
                .ForMember(dest => dest.TipoPropiedad, opt => opt.MapFrom(src => src.TipoPropiedad.Nombre))
                .ForMember(dest => dest.TipoVenta, opt => opt.MapFrom(src => src.TipoVenta.Nombre))
                .ForMember(dest => dest.NombreAgente, opt => opt.Ignore())
                .ForMember(dest => dest.IdAgente, opt => opt.MapFrom(src => src.AgenteId))
                .ForMember(dest => dest.Mejoras, opt => opt.MapFrom(src => src.PropiedadesMejoras.Select(pm => pm.Mejora.Nombre).ToList()))
                .ForMember(dest => dest.EstadoPropiedad, opt => opt.MapFrom(src => src.Estado == EstadoPropiedad.Disponible ? "Disponible" : "Vendida"))
                .ForMember(dest => dest.TamanoEnMetros, opt => opt.MapFrom(src => src.TamanoEnMetros)) 
                .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => src.FechaCreacion));
        }
    }
}