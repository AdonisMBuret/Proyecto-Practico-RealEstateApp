using AutoMapper;
using RealEstateApp.Application.ViewModels.Ofertas;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Application.Mappings.DtosAndViewModels;

public class OfertasViewModelProfile : Profile
{
    public OfertasViewModelProfile()
    {
        CreateMap<Oferta, OfertaViewModel>()
            .ForMember(dest => dest.MontoOferta, opt => opt.MapFrom(src => src.Monto))
            .ForMember(dest => dest.CodigoPropiedad, opt => opt.MapFrom(src => 
                src.Propiedad != null ? src.Propiedad.Codigo : "N/A"))
            .ForMember(dest => dest.PropiedadCodigo, opt => opt.MapFrom(src => 
                src.Propiedad != null ? src.Propiedad.Codigo : null))
            .ForMember(dest => dest.PropiedadDescripcion, opt => opt.MapFrom(src => 
                src.Propiedad != null ? src.Propiedad.Descripcion : null))
            .ForMember(dest => dest.ClienteNombre, opt => opt.Ignore())
            .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => (int)src.Estado))
            .ForMember(dest => dest.EstadoTexto, opt => opt.MapFrom(src => src.Estado.ToString()));


        CreateMap<SaveOfertaViewModel, Oferta>()
            .ForMember(dest => dest.Monto, opt => opt.MapFrom(src => src.MontoOferta))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Estado, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.Propiedad, opt => opt.Ignore());
    }
}