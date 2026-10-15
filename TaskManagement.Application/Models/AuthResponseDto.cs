namespace TaskManagement.Application.Models;
public class AuthResponseDto
{
     
      public int UserId { get; set; } 
     public string Name{get; set;} =string.Empty;
     public string Email{get;set;} = string.Empty;
 
    public string AccessToken{get; set;}= string.Empty;

     public DateTime ExpiresAtUtc { get; set; } // when the JWT stops being valid




}