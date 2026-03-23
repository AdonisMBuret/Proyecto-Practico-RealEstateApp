namespace RealEstateApp.Application.Interfaces.Services;


public interface IJwtService
{
    string GenerateToken(string userId, string email, string username, List<string> roles);
}
