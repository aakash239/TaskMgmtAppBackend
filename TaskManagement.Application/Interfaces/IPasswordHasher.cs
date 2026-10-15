namespace TaskManagement.Application.Interfaces;

// Contract for hashing passwords. Implemented in Infrastructure.
public interface IPasswordHasher
{
    string Hash(string password);                          // plain text -> hash
    bool Verify(string password, string passwordHash);     // used later by login
}