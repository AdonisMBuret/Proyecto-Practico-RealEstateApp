using RealEstateApp.Domain.Common;

namespace RealEstateApp.Domain.Entities;

public class ImagenPropiedad : BaseEntity
{
    public string UrlImagen { get; set; } = null!;
    public bool EsPrincipal { get; set; }

    public int PropiedadId { get; set; }

    public Propiedad Propiedad { get; set; } = null!;
}