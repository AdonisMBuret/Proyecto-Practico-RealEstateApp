using AutoMapper;
using RealEstateApp.Application.DTOs.Mantenimientos;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Application.Mappings
{
    public class MantenimientoApiProfile : Profile
    {
        public MantenimientoApiProfile()
        {

            CreateMap<TipoPropiedad, TipoPropiedadApiDTO>()
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
                .ForMember(dest => dest.FechaModificacion, opt => opt.Ignore());


            CreateMap<TipoVenta, TipoVentaApiDTO>()
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
                .ForMember(dest => dest.FechaModificacion, opt => opt.Ignore());

            CreateMap<Mejora, MejoraApiDTO>()
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
                .ForMember(dest => dest.FechaModificacion, opt => opt.Ignore());
        }
    }
}