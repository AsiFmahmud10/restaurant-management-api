namespace ProductManagement.Exception;

public class UnAuthorizedException(string message) : System.Exception
{
    public string Message { get; set; } = message;
}