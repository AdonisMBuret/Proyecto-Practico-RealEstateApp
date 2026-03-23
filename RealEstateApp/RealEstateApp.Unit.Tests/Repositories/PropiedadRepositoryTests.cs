using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Enums;
using RealEstateApp.Persistence.Contexts;
using RealEstateApp.Persistence.Repositories;
using AutoMapper;
using Moq;
using Xunit;
namespace RealEstateApp.Unit.Tests.Repositories;
public class PropiedadRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly PropiedadRepository _repository;
    private readonly Mock<IMapper> _mockMapper;
    public PropiedadRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;
        _context = new ApplicationDbContext(options);
        _mockMapper = new Mock<IMapper>();
        _repository = new PropiedadRepository(_context, _mockMapper.Object);
        _context.Database.EnsureCreated();
    }
    [Fact]
    public async Task GetAllDisponiblesAsync_Should_ReturnOnlyAvailableProperties()
    {
        await SeedTestDataAsync();
        var result = await _repository.GetAllDisponiblesAsync();
        result.Should().NotBeNull();
        result.Should().HaveCount(2); 
        result.All(p => p.Estado == EstadoPropiedad.Disponible).Should().BeTrue();
    }
    [Fact]
    public async Task GetByCodigoAsync_When_PropertyExists_Should_ReturnProperty()
    {
        await SeedTestDataAsync();
        var codigo = "PROP001";
        var result = await _repository.GetByCodigoAsync(codigo);
        result.Should().NotBeNull();
        result!.Codigo.Should().Be(codigo);
        result.TipoPropiedad.Should().NotBeNull(); 
    }
    [Fact]
    public async Task GetByCodigoAsync_When_PropertyNotExists_Should_ReturnNull()
    {
        await SeedTestDataAsync();
        var result = await _repository.GetByCodigoAsync("NONEXISTENT");
        result.Should().BeNull();
    }
    [Fact]
    public async Task ExisteCodigoAsync_When_CodeExists_Should_ReturnTrue()
    {
        await SeedTestDataAsync();
        var result = await _repository.ExisteCodigoAsync("PROP001");
        result.Should().BeTrue();
    }
    [Fact]
    public async Task ExisteCodigoAsync_When_CodeNotExists_Should_ReturnFalse()
    {
        await SeedTestDataAsync();
        var result = await _repository.ExisteCodigoAsync("NONEXISTENT");
        result.Should().BeFalse();
    }
    [Fact]
    public async Task ExisteCodigoAsync_When_ExcludingId_Should_IgnoreSpecificProperty()
    {
        await SeedTestDataAsync();
        var excludeId = 1; 
        var result = await _repository.ExisteCodigoAsync("PROP001", excludeId);
        result.Should().BeFalse(); 
    }
    [Fact]
    public async Task AddAsync_Should_CreatePropertyWithGeneratedId()
    {
        await SeedBaseDataAsync(); 
        var newPropiedad = new Propiedad
        {
            Codigo = "PROP999",
            Precio = 2_000_000m,
            Descripcion = "Nueva propiedad",
            Estado = EstadoPropiedad.Disponible,
            AgenteId = "agente-123",
            TipoPropiedadId = 1,
            TipoVentaId = 1,
            CantidadHabitaciones = 2,
            CantidadBanos = 1,
            TamanoEnMetros = 75,
            FechaCreacion = DateTime.UtcNow
        };
        var result = await _repository.AddAsync(newPropiedad);
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Codigo.Should().Be("PROP999");
        var savedProperty = await _context.Propiedades.FindAsync(result.Id);
        savedProperty.Should().NotBeNull();
    }
    [Fact]
    public async Task UpdateAsync_Should_ModifyExistingProperty()
    {
        await SeedTestDataAsync();
        var propiedad = await _repository.GetByIdAsync(1);
        propiedad!.Precio = 3_500_000m;
        propiedad.Descripcion = "Descripción actualizada";
        await _repository.UpdateAsync(propiedad);
        var updatedProperty = await _repository.GetByIdAsync(1);
        updatedProperty.Should().NotBeNull();
        updatedProperty!.Precio.Should().Be(3_500_000m);
        updatedProperty.Descripcion.Should().Be("Descripción actualizada");
    }
    [Fact]
    public async Task DeleteAsync_Should_RemoveProperty()
    {
        await SeedTestDataAsync();
        var propiedad = await _repository.GetByIdAsync(1);
        await _repository.DeleteAsync(propiedad!);
        var deletedProperty = await _repository.GetByIdAsync(1);
        deletedProperty.Should().BeNull();
    }
    [Fact]
    public async Task GenerarCodigoAsync_Should_ReturnUniqueCode()
    {
        await SeedTestDataAsync(); 
        var newCodigo = await _repository.GenerarCodigoAsync();
        newCodigo.Should().NotBeNullOrEmpty();
        newCodigo.Should().StartWith("PROP");
        newCodigo.Should().Be("PROP004"); 
    }
    [Fact]
    public async Task GetEstadisticasAsync_Should_ReturnCorrectCounts()
    {
        await SeedTestDataAsync(); 
        var (disponibles, vendidas) = await _repository.GetEstadisticasAsync();
        disponibles.Should().Be(2);
        vendidas.Should().Be(1);
    }
    [Fact]
    public async Task EstaDisponibleAsync_When_PropertyAvailable_Should_ReturnTrue()
    {
        await SeedTestDataAsync();
        var result = await _repository.EstaDisponibleAsync(1); 
        result.Should().BeTrue();
    }
    [Fact]
    public async Task EstaDisponibleAsync_When_PropertySold_Should_ReturnFalse()
    {
        await SeedTestDataAsync();
        var result = await _repository.EstaDisponibleAsync(3); 
        result.Should().BeFalse();
    }
    private async Task SeedBaseDataAsync()
    {
        if (_context.Propiedades.Any())
            _context.RemoveRange(_context.Propiedades);
        if (_context.TiposVentas.Any())
            _context.RemoveRange(_context.TiposVentas);
        if (_context.TiposPropiedades.Any())
            _context.RemoveRange(_context.TiposPropiedades);
        await _context.SaveChangesAsync();
        var tipoPropiedad = new TipoPropiedad { Id = 1, Nombre = "Casa", Descripcion = "Casa familiar" };
        var tipoVenta = new TipoVenta { Id = 1, Nombre = "Venta", Descripcion = "Venta directa" };
        _context.TiposPropiedades.Add(tipoPropiedad);
        _context.TiposVentas.Add(tipoVenta);
        await _context.SaveChangesAsync();
    }
    private async Task SeedTestDataAsync()
    {
        await SeedBaseDataAsync();
        var propiedades = new List<Propiedad>
        {
            new() {
                Id = 1,
                Codigo = "PROP001",
                Precio = 2_500_000m,
                Descripcion = "Casa en Santo Domingo",
                Estado = EstadoPropiedad.Disponible,
                AgenteId = "agente-123",
                TipoPropiedadId = 1,
                TipoVentaId = 1,
                CantidadHabitaciones = 3,
                CantidadBanos = 2,
                TamanoEnMetros = 120,
                FechaCreacion = DateTime.UtcNow.AddDays(-30)
            },
            new() {
                Id = 2,
                Codigo = "PROP002",
                Precio = 1_800_000m,
                Descripcion = "Apartamento en Santiago",
                Estado = EstadoPropiedad.Disponible,
                AgenteId = "agente-456",
                TipoPropiedadId = 1,
                TipoVentaId = 1,
                CantidadHabitaciones = 2,
                CantidadBanos = 1,
                TamanoEnMetros = 85,
                FechaCreacion = DateTime.UtcNow.AddDays(-15)
            },
            new() {
                Id = 3,
                Codigo = "PROP003",
                Precio = 4_500_000m,
                Descripcion = "Villa en Punta Cana",
                Estado = EstadoPropiedad.Vendida,
                AgenteId = "agente-789",
                TipoPropiedadId = 1,
                TipoVentaId = 1,
                CantidadHabitaciones = 5,
                CantidadBanos = 4,
                TamanoEnMetros = 250,
                FechaCreacion = DateTime.UtcNow.AddDays(-60)
            }
        };
        _context.Propiedades.AddRange(propiedades);
        await _context.SaveChangesAsync();
    }
    public void Dispose()
    {
        _context?.Database.EnsureDeleted();
        _context?.Dispose();
    }
}