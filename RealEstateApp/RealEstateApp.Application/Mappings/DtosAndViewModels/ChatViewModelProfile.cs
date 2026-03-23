using AutoMapper;
using RealEstateApp.Application.ViewModels.Chat;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Application.Mappings.DtosAndViewModels;

public class ChatViewModelProfile : Profile
{
    public ChatViewModelProfile()
    {
        CreateMap<Mensaje, MensajeViewModel>()
            .ForMember(dest => dest.CodigoPropiedad, opt => opt.Ignore())
            .ForMember(dest => dest.EmisorNombre, opt => opt.Ignore())
            .ForMember(dest => dest.ReceptorNombre, opt => opt.Ignore())
            .ForMember(dest => dest.EsMio, opt => opt.Ignore());

        CreateMap<SaveMensajeViewModel, Mensaje>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.FechaEnvio, opt => opt.Ignore())
            .ForMember(dest => dest.EsLeido, opt => opt.Ignore())
            .ForMember(dest => dest.ChatId, opt => opt.Ignore())
            .ForMember(dest => dest.Chat, opt => opt.Ignore());

        CreateMap<Mensaje, ChatViewModel>();

        CreateMap<Chat, ConversacionViewModel>()
            .ForMember(dest => dest.CodigoPropiedad, opt => opt.MapFrom(src => 
                src.Propiedad != null ? src.Propiedad.Codigo : "N/A"))
            .ForMember(dest => dest.ClienteNombre, opt => opt.Ignore()) 
            .ForMember(dest => dest.UltimoMensaje, opt => opt.MapFrom(src => 
                src.Mensajes.Any() ? src.Mensajes.OrderByDescending(m => m.FechaEnvio).First().Contenido : "Sin mensajes"))
            .ForMember(dest => dest.FechaUltimoMensaje, opt => opt.MapFrom(src => 
                src.Mensajes.Any() ? src.Mensajes.OrderByDescending(m => m.FechaEnvio).First().FechaEnvio : src.FechaCreacion))
            .ForMember(dest => dest.MensajesNoLeidos, opt => opt.MapFrom(src => 
                src.Mensajes.Count(m => !m.EsLeido && m.EmisorId == src.ClienteId)))
            .ForMember(dest => dest.TotalMensajes, opt => opt.MapFrom(src => src.Mensajes.Count))
            .ForMember(dest => dest.EsConversacionActiva, opt => opt.MapFrom(src => 
                src.Mensajes.Any(m => m.FechaEnvio >= DateTime.UtcNow.AddDays(-30))));
    }
}