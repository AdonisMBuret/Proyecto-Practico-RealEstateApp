using AutoMapper;
using RealEstateApp.Application.ViewModels.Agentes;
using RealEstateApp.Application.DTOs.Agentes;


namespace RealEstateApp.Application.Mappings.DtosAndViewModels;


public class AgentesViewModelProfile : Profile
{
    public AgentesViewModelProfile()
    {
        CreateMap<AgenteDTO, AgenteViewModel>()
            .ForMember(dest => dest.NombreCompleto, opt => opt.MapFrom(src => $"{src.Nombre} {src.Apellido}"))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Correo))
            .ForMember(dest => dest.CantidadPropiedades, opt => opt.MapFrom(src => src.CantidadPropiedades));

        CreateMap<AgenteDTO, AgentePerfilViewModel>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Correo));

        CreateMap<AgenteDTO, EditarAgenteViewModel>()
            .ForMember(dest => dest.FotoActual, opt => opt.Ignore())
            .ForMember(dest => dest.NuevaFoto, opt => opt.Ignore());

        CreateMap<EditarAgenteViewModel, AgenteDTO>()
            .ForMember(dest => dest.Correo, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.CantidadPropiedades, opt => opt.Ignore());

        CreateMap<List<AgenteDTO>, ListadoAgentesViewModel>()
            .ForMember(dest => dest.Agentes, opt => opt.MapFrom(src => src))
            .ForMember(dest => dest.NombreBusqueda, opt => opt.Ignore());
    }
}