namespace ProductManagement.Exception;

public class ConflictException(string message) : System.Exception(message)
{
    private string Message { get; set; } = message;
}