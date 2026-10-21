namespace TaskManagement.Core.Exceptions;


public class ConflictException : Exception
{
    // Pass the human-readable message up to the base Exception.
    public ConflictException(string message) : base(message) { }
}