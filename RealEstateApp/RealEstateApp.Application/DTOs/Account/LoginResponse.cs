namespace RealEstateApp.Application.DTOs.Account;

public class LoginResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Token { get; set; }
    public string? UserId { get; set; }
    public string? Email { get; set; }
    public string? Username { get; set; }
    public string? FullName { get; set; }
    public List<string> Roles { get; set; } = new();
}
