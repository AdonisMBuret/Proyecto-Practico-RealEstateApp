namespace RealEstateApp.Application.ViewModels.Chat;

public class ConversacionConPropiedadViewModel
{
    public ConversacionViewModel Conversacion { get; set; } = new();
    public string PropiedadCodigo { get; set; } = string.Empty;
    public string PropiedadDescripcion { get; set; } = string.Empty;
}
