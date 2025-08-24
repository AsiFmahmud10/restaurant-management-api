namespace ProductManagement.Exception;

public class UnAuthorizedException(string message) : System.Exception
{
    private string Message { get; set; } = message;

}