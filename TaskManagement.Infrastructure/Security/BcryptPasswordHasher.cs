using TaskManagement.Application.Interfaces;

namespace TaskManagement.Infrastructure.Security;

// BCrypt is a salted hash designed for passwords. The salt is embedded in the output.
public class BcryptPasswordHasher : IPasswordHasher
{
    // Work factor 12 : Higher is slower and safer.
    private const int WorkFactor = 12;

    public string Hash(string password) =>
        BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);

    //  re-hashes the input with the stored salt and compares.
    public bool Verify(string password, string passwordHash) =>
        BCrypt.Net.BCrypt.Verify(password, passwordHash);
}