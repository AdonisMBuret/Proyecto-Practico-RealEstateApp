using AutoMapper;
using RealEstateApp.Application.DTOs.Agentes;
using RealEstateApp.Application.ViewModels.Agentes;

namespace RealEstateApp.Application.Mappings
{
    public class AgenteApiProfile : Profile
    {
        public AgenteApiProfile()
        {
            CreateMap<AgenteViewModel, AgenteApiDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Apellido, opt => opt.MapFrom(src => src.Apellido))
                .ForMember(dest => dest.Correo, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Telefono, opt => opt.MapFrom(src => src.Telefono ?? string.Empty))
                .ForMember(dest => dest.EsActivo, opt => opt.Ignore())
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
                .ForMember(dest => dest.CantidadPropiedades, opt => opt.Ignore());
        }
    }
}