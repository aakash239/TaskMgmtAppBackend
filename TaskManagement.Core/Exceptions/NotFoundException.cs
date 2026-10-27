namespace TaskManagement.Core.Exceptions;

public class NotFoundException: Exception
{
    public NotFoundException(string message) : base(message) {}
    //passing the message to the base exception class 
}