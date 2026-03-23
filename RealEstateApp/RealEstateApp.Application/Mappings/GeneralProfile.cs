using AutoMapper;
using RealEstateApp.Application.ViewModels.Propiedades;
using RealEstateApp.Application.ViewModels.Catalogos;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Application.Mappings;

public class GeneralProfile : Profile
{
    public GeneralProfile()
    {
        #region Propiedad Mappings
        
        CreateMap<Propiedad, PropiedadViewModel>()
            .ForMember(dest => dest.TipoPropiedad, opt => opt.MapFrom(src => src.TipoPropiedad.Nombre))
            .ForMember(dest => dest.TipoVenta, opt => opt.MapFrom(src => src.TipoVenta.Nombre))
            .ForMember(dest => dest.EstadoTexto, opt => opt.MapFrom(src => src.Estado.ToString()))
            .ForMember(dest => dest.ImagenPrincipal, opt => opt.MapFrom(src => 
                src.Imagenes.FirstOrDefault() != null ? src.Imagenes.FirstOrDefault()!.UrlImagen : null))
            .ForMember(dest => dest.AgenteId, opt => opt.MapFrom(src => src.AgenteId))
            .ForMember(dest => dest.AgenteNombre, opt => opt.Ignore()) 
            .ForMember(dest => dest.AgenteTelefono, opt => opt.Ignore())
            .ForMember(dest => dest.AgenteEmail, opt => opt.Ignore())
            .ForMember(dest => dest.AgenteFoto, opt => opt.Ignore())
            .ReverseMap()
            .ForMember(dest => dest.TipoPropiedad, opt => opt.Ignore())
            .ForMember(dest => dest.TipoVenta, opt => opt.Ignore())
            .ForMember(dest => dest.Imagenes, opt => opt.Ignore())
            .ForMember(dest => dest.PropiedadesMejoras, opt => opt.Ignore())
            .ForMember(dest => dest.PropiedadesFavoritas, opt => opt.Ignore())
            .ForMember(dest => dest.Ofertas, opt => opt.Ignore())
            .ForMember(dest => dest.Chats, opt => opt.Ignore());

        #endregion

        #region TipoPropiedad Mappings
        
        CreateMap<TipoPropiedad, ViewModels.Catalogos.TipoPropiedadViewModel>().ReverseMap();
        
        #endregion

        #region TipoVenta Mappings
        
        CreateMap<TipoVenta, ViewModels.Catalogos.TipoVentaViewModel>().ReverseMap();
        
        #endregion

        #region Mejora Mappings
        
        CreateMap<Mejora, ViewModels.Catalogos.MejoraViewModel>().ReverseMap();
        
        #endregion
    }
}
