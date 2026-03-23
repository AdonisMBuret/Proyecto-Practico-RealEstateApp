using AutoMapper;
using FluentAssertions;
using Moq;
using RealEstateApp.Application.Features.Propiedades.Queries.GetPropiedadById;
using RealEstateApp.Application.ViewModels.Propiedades;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Enums;
using RealEstateApp.Domain.Interfaces;
using Xunit;
namespace RealEstateApp.Unit.Tests.Features.Queries;
public class GetPropiedadByIdQueryTests
{
    private readonly Mock<IPropiedadRepository> _mockPropiedadRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly GetPropiedadByIdQueryHandler _handler;
    public GetPropiedadByIdQueryTests()
    {
        _mockPropiedadRepository = new Mock<IPropiedadRepository>();
        _mockMapper = new Mock<IMapper>();
        _handler = new GetPropiedadByIdQueryHandler(
            _mockPropiedadRepository.Object,
            _mockMapper.Object
        );
    }
    #region GetPropiedadById Tests
    [Fact]
    public async Task Handle_When_PropiedadExists_Should_ReturnPropiedadSuccessfully()
    {
        var propiedadId = 1;
        var query = new GetPropiedadByIdQuery(propiedadId);
        var propiedad = CreateTestPropiedad(propiedadId);
        var expectedViewModel = CreateTestPropiedadViewModel(propiedad);
        _mockPropiedadRepository
            .Setup(x => x.GetByIdWithDetailsAsync(propiedadId))
            .ReturnsAsync(propiedad);
        _mockMapper
            .Setup(x => x.Map<PropiedadViewModel>(propiedad))
            .Returns(expectedViewModel);
        var result = await _handler.Handle(query, CancellationToken.None);
        result.Should().NotBeNull();
        result!.Id.Should().Be(propiedadId);
        result.Codigo.Should().Be("PROP001");
        result.Precio.Should().Be(5_200_000m);
        _mockPropiedadRepository.Verify(x => x.GetByIdWithDetailsAsync(propiedadId), Times.Once);
        _mockMapper.Verify(x => x.Map<PropiedadViewModel>(propiedad), Times.Once);
    }
    [Fact]
    public async Task Handle_When_PropiedadNotFound_Should_ReturnNull()
    {
        var propiedadId = 999;
        var query = new GetPropiedadByIdQuery(propiedadId);
        _mockPropiedadRepository
            .Setup(x => x.GetByIdWithDetailsAsync(propiedadId))
            .ReturnsAsync((Propiedad?)null);
        var result = await _handler.Handle(query, CancellationToken.None);
        result.Should().BeNull();
        _mockPropiedadRepository.Verify(x => x.GetByIdWithDetailsAsync(propiedadId), Times.Once);
        _mockMapper.Verify(x => x.Map<PropiedadViewModel>(It.IsAny<Propiedad>()), Times.Never);
    }
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-999)]
    public async Task Handle_When_InvalidId_Should_ReturnNull(int invalidId)
    {
        var query = new GetPropiedadByIdQuery(invalidId);
        _mockPropiedadRepository
            .Setup(x => x.GetByIdWithDetailsAsync(invalidId))
            .ReturnsAsync((Propiedad?)null);
        var result = await _handler.Handle(query, CancellationToken.None);
        result.Should().BeNull();
        _mockPropiedadRepository.Verify(x => x.GetByIdWithDetailsAsync(invalidId), Times.Once);
    }
    [Fact]
    public async Task Handle_When_RepositoryThrowsException_Should_PropagateException()
    {
        var propiedadId = 1;
        var query = new GetPropiedadByIdQuery(propiedadId);
        _mockPropiedadRepository
            .Setup(x => x.GetByIdWithDetailsAsync(propiedadId))
            .ThrowsAsync(new InvalidOperationException("Database connection error"));
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(query, CancellationToken.None));
        exception.Message.Should().Be("Database connection error");
        _mockPropiedadRepository.Verify(x => x.GetByIdWithDetailsAsync(propiedadId), Times.Once);
    }
    #endregion
    #region Performance Tests
    [Fact]
    public async Task Handle_Should_CompleteWithinAcceptableTime()
    {
        var propiedadId = 1;
        var query = new GetPropiedadByIdQuery(propiedadId);
        var propiedad = CreateTestPropiedad(propiedadId);
        var expectedViewModel = CreateTestPropiedadViewModel(propiedad);
        _mockPropiedadRepository
            .Setup(x => x.GetByIdWithDetailsAsync(propiedadId))
            .ReturnsAsync(propiedad);
        _mockMapper
            .Setup(x => x.Map<PropiedadViewModel>(propiedad))
            .Returns(expectedViewModel);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await _handler.Handle(query, CancellationToken.None);
        stopwatch.Stop();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000); 
        result.Should().NotBeNull();
    }
    #endregion
    #region Helper Methods
    private static Propiedad CreateTestPropiedad(int id) => new()
    {
        Id = id,
        Codigo = "PROP001",
        Precio = 5_200_000m,
        Descripcion = "Apartamento en la Capital con excelente ubicación",
        CantidadHabitaciones = 3,
        CantidadBanos = 2,
        TamanoEnMetros = 95,
        AgenteId = "agente-123",
        TipoPropiedadId = 1,
        TipoVentaId = 1,
        Estado = EstadoPropiedad.Disponible,
        FechaCreacion = DateTime.UtcNow
    };
    private static PropiedadViewModel CreateTestPropiedadViewModel(Propiedad propiedad) => new()
    {
        Id = propiedad.Id,
        Codigo = propiedad.Codigo,
        Precio = propiedad.Precio,
        Descripcion = propiedad.Descripcion,
        CantidadHabitaciones = propiedad.CantidadHabitaciones,
        CantidadBanos = propiedad.CantidadBanos,
        TamanoEnMetros = propiedad.TamanoEnMetros,
        EstadoTexto = propiedad.Estado.ToString(),
        FechaCreacion = propiedad.FechaCreacion
    };
    #endregion
}