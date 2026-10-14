namespace TaskManagement.Application.Models;
public class AuthResponseDto
{
     public string Name{get; set;} =string.Empty;
     public string Email{get;set;} = string.Empty;
 
    public string AccessToken{get; set;}= string.Empty;

    public int ExpiresIn {get; set;}




}