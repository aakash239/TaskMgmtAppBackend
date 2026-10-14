namespace TaskManagement.Application.Interfaces;

public interface IPasswordService
{
    string HashPassword (string password); // use this to store password in database

    bool VerifyPassword (string hashedPassword, string providedPassword);
}