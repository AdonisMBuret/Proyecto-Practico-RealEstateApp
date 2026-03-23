using AutoMapper;
using RealEstateApp.Application.ViewModels.Propiedades;
using RealEstateApp.Application.ViewModels.Agentes;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Application.Mappings.DtosAndViewModels;

public class PropiedadesViewModelProfile : Profile
{
    public PropiedadesViewModelProfile()
    {
        CreateMap<Propiedad, PropiedadViewModel>()
            .ForMember(dest => dest.TipoPropiedad, opt => opt.MapFrom(src => 
                src.TipoPropiedad != null ? src.TipoPropiedad.Nombre : "N/A"))
            .ForMember(dest => dest.TipoVenta, opt => opt.MapFrom(src => 
                src.TipoVenta != null ? src.TipoVenta.Nombre : "N/A"))
            .ForMember(dest => dest.EstadoTexto, opt => opt.MapFrom(src => src.Estado.ToString()))
            .ForMember(dest => dest.AgenteId, opt => opt.MapFrom(src => src.AgenteId))
            .ForMember(dest => dest.AgenteNombre, opt => opt.Ignore())
            .ForMember(dest => dest.AgenteEmail, opt => opt.Ignore())
            .ForMember(dest => dest.AgenteTelefono, opt => opt.Ignore())
            .ForMember(dest => dest.AgenteFoto, opt => opt.Ignore())
            .ForMember(dest => dest.ImagenPrincipal, opt => opt.MapFrom(src => 
                src.Imagenes.FirstOrDefault() != null ? src.Imagenes.FirstOrDefault()!.UrlImagen : null))
            .ForMember(dest => dest.Mejoras, opt => opt.MapFrom(src => 
                src.PropiedadesMejoras != null 
                    ? src.PropiedadesMejoras.Where(pm => pm.Mejora != null).Select(pm => pm.Mejora.Nombre).ToList()
                    : new List<string>()));
                
        CreateMap<Propiedad, PropiedadDetalleViewModel>()
            .ForMember(dest => dest.TipoPropiedad, opt => opt.MapFrom(src => 
                src.TipoPropiedad != null ? src.TipoPropiedad.Nombre : "N/A"))
            .ForMember(dest => dest.TipoVenta, opt => opt.MapFrom(src => 
                src.TipoVenta != null ? src.TipoVenta.Nombre : "N/A"))
            .ForMember(dest => dest.Imagenes, opt => opt.MapFrom(src => 
                src.Imagenes.Select(i => i.UrlImagen).ToList()))
            .ForMember(dest => dest.Mejoras, opt => opt.MapFrom(src => 
                src.PropiedadesMejoras != null
                    ? src.PropiedadesMejoras.Where(pm => pm.Mejora != null).Select(pm => pm.Mejora.Nombre).ToList()
                    : new List<string>()))
            .ForMember(dest => dest.Agente, opt => opt.Ignore()) 
            .ForMember(dest => dest.EstaDisponible, opt => opt.MapFrom(src => 
                src.Estado == Domain.Enums.EstadoPropiedad.Disponible));

        CreateMap<Propiedad, SavePropiedadViewModel>()
            .ForMember(dest => dest.MejorasSeleccionadas, opt => opt.MapFrom(src => 
                src.PropiedadesMejoras != null ? src.PropiedadesMejoras.Select(pm => pm.MejoraId).ToList() : new List<int>()))
            .ForMember(dest => dest.ImagenesActuales, opt => opt.MapFrom(src => 
                src.Imagenes.Select(i => i.UrlImagen).ToList()))
            .ForMember(dest => dest.Imagenes, opt => opt.Ignore())
            .ForMember(dest => dest.ImagenesAEliminar, opt => opt.Ignore());

        CreateMap<SavePropiedadViewModel, Propiedad>()
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

        CreateMap<FiltrosPropiedadesViewModel, HomeViewModel>()
            .ForMember(dest => dest.Filtros, opt => opt.MapFrom(src => src))
            .ForMember(dest => dest.Propiedades, opt => opt.Ignore())
            .ForMember(dest => dest.ClienteId, opt => opt.Ignore())
            .ForMember(dest => dest.PropiedadesFavoritas, opt => opt.Ignore());
    }
}