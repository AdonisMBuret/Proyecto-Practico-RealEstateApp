namespace RealEstateApp.Application.Features.Propiedades.Commands.DeletePropiedad;


public class DeletePropiedadResponse
{
    public bool Success { get; set; } = true;
    public string Mensaje { get; set; } = "Propiedad eliminada exitosamente";
}
