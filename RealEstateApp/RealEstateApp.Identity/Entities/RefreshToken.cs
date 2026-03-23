namespace RealEstateApp.Identity.Entities;


public class RefreshToken
{

    public int Id { get; set; }

    public string Token { get; set; } = string.Empty;

    public DateTime ExpiresOn { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime? RevokedOn { get; set; }

    
    public bool IsActive => RevokedOn == null && !IsExpired;

 
    public bool IsExpired => DateTime.UtcNow >= ExpiresOn;


    public string UserId { get; set; } = string.Empty;


    public virtual ApplicationUser User { get; set; } = null!;

    public RefreshToken()
    {
        CreatedOn = DateTime.UtcNow;
    }
}
