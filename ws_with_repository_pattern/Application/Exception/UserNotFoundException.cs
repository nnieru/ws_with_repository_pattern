namespace ws_with_repository_pattern.Application.Exception;

public class UserNotFoundException: System.Exception
{
    public UserNotFoundException()
    {
        
    }
    
    public UserNotFoundException(string message): base(message)
    {
        
    }
    
    
}