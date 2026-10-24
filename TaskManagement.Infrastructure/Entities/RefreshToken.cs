namespace TaskManagement.Infrastructure.Entities;

public class RefreshToken
{
    public long RefreshTokenId  {get; set;}
    public Guid SessionId {get; set;} 
    public string TokenHash {get; set;} = string.Empty;

    public DateTime CreatedAt{get; set;}
    public DateTime ExpiresAt{get; set;}
    public DateTime? RevokedAt{get; set;}
    public long? ReplacedByTokenId {get; set;}

    //Navigation Properties
    public UserSession Session {get; set;} = null!;
    public RefreshToken? ReplacedByToken {get; set;}
}