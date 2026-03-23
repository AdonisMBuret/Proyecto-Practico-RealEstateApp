using Microsoft.EntityFrameworkCore;
using RealEstateApp.Domain.Entities;
using System.Reflection;

namespace RealEstateApp.Persistence.Contexts;


public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Propiedad> Propiedades { get; set; }
    public DbSet<TipoPropiedad> TiposPropiedades { get; set; }
    public DbSet<TipoVenta> TiposVentas { get; set; }
    public DbSet<Mejora> Mejoras { get; set; }
    public DbSet<ImagenPropiedad> ImagenesPropiedades { get; set; }
    public DbSet<PropiedadMejora> PropiedadesMejoras { get; set; }
    public DbSet<PropiedadFavorita> PropiedadesFavoritas { get; set; }
    public DbSet<Oferta> Ofertas { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<Mensaje> Mensajes { get; set; }

   
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        
    }

    
}
