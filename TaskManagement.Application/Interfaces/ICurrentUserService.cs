namespace TaskManagement.Application.Interfaces;

// gives services the id of the logged- in user 
// to be implemented in the Api project 
public interface ICurrentUserService
{
    int UserId {get;}
}