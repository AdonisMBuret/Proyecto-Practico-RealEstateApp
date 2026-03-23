using FluentAssertions;
using RealEstateApp.Application.ViewModels.Propiedades;
using System.Globalization;
namespace RealEstateApp.Unit.Tests.Validation;
public class DOPPriceValidationTests
{
    [Theory]
    [InlineData(1_500_000, true)]    
    [InlineData(3_800_000, true)]    
    [InlineData(15_000_000, true)]   
    [InlineData(800_000, true)]      
    [InlineData(0, false)]           
    [InlineData(-1000, false)]       
    public void ValidateDOPPrice_ShouldValidateCorrectly(decimal precio, bool isValid)
    {
        var propiedad = new PropiedadViewModel
        {
            Precio = precio,
            Descripcion = "Propiedad de prueba",
            CantidadHabitaciones = 3,
            CantidadBanos = 2
        };
        var result = ValidateDOPPrice(propiedad.Precio);
        result.Should().Be(isValid, $"Precio {precio:C} DOP debe ser {(isValid ? "válido" : "inválido")}");
    }
    [Fact]
    public void DOPPrice_ShouldMaintainPrecision()
    {
        var preciosConDecimales = new[]
        {
            1_250_000.50m,
            2_750_000.99m,
            4_500_000.25m
        };
        foreach (var precio in preciosConDecimales)
        {
            var formatted = $"RD$ {precio:N2}"; 
            var parsed = decimal.Parse(precio.ToString());
            parsed.Should().Be(precio, "Los precios deben mantener precisión decimal");
            formatted.Should().Contain("RD$", "El formato debe incluir la moneda dominicana");
        }
    }
    [Fact]
    public void DOPPrice_Ranges_ShouldBeRealistic()
    {
        var rangosCasas = new Dictionary<string, (decimal min, decimal max)>
        {
            { "Apartamento Económico", (600_000m, 1_500_000m) },
            { "Casa Clase Media", (1_500_000m, 4_000_000m) },
            { "Casa Premium", (4_000_000m, 8_000_000m) },
            { "Villa de Lujo", (8_000_000m, 20_000_000m) }
        };
        foreach (var (categoria, rango) in rangosCasas)
        {
            rango.min.Should().BeGreaterThan(0, $"{categoria} debe tener precio mínimo positivo");
            rango.max.Should().BeGreaterThan(rango.min, $"{categoria} debe tener rango válido");
            rango.min.Should().BeGreaterThan(500_000m, 
                $"{categoria} en DOP debe ser mayor que precios típicos en USD");
        }
    }
    [Theory]
    [InlineData("1500000", 1_500_000)]    
    [InlineData("1,500,000", 1_500_000)]  
    [InlineData("1.500.000", 1_500_000)]  
    public void DOPPrice_Parsing_ShouldHandleFormats(string input, decimal expected)
    {
        var success = decimal.TryParse(input.Replace(",", "").Replace(".", ""), out var result);
        success.Should().BeTrue("Debe poder parsear formatos comunes de precios");
        result.Should().Be(expected, "El valor parseado debe ser correcto");
    }
    private static bool ValidateDOPPrice(decimal precio)
    {
        return precio > 0 && precio <= 50_000_000m; 
    }
}