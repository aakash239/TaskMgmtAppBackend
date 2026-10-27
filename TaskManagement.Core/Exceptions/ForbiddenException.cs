namespace TaskManagement.Core.Exceptions;

public class ForbiddenException: Exception
{
    
    public ForbiddenException (string message) : base(message){ }
    // passing the message to the base Exception class 
}