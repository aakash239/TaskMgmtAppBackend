namespace TaskManagement.Infrastructure.Entities;

public class UserSession
{
    public Guid SessionId {get; set;}
    public int UserId {get; set;}
    public DateTime CreatedAt{get; set;}
    public DateTime ExpiresAt{get; set;}
    public DateTime? LastActivityAt{get; set;}
    public DateTime? RevokedAt{get; set;}

    // Navigation Properties: describe relatinships to efCore
    public User User {get; set;} = null!;
    public ICollection<RefreshToken> RefreshTokens {get; set;} = new List<RefreshToken>(); 
}