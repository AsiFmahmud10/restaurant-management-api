namespace ProductManagement.Exception;

public class BadRequestException(string message) : System.Exception(message)
{
    private string Message { get; set; } = message;
}