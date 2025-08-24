namespace ProductManagement.Exception;

public class ResourceNotFoundException(string message) : System.Exception(message)
{
    private string Message { get; set; } = message;
}