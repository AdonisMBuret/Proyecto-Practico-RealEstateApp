using RealEstateApp.Domain.Common;

namespace RealEstateApp.Domain.Entities;

public class Mensaje : BaseEntity
{
    public string Contenido { get; set; } = null!;
    public DateTime FechaEnvio { get; set; }
    public bool EsLeido { get; set; }

    public int ChatId { get; set; }
    public string EmisorId { get; set; } = null!;
    public string ReceptorId { get; set; } = null!;

    public Chat Chat { get; set; } = null!;
}