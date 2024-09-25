namespace ws_with_repository_pattern.Infrastructures.Helper;

public class Result<T>
{
    public T? Data { get; set; }
    public List<string> ErrorMessages { get; set; }

}