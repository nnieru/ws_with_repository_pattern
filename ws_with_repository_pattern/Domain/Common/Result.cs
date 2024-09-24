namespace ws_with_repository_pattern.Domain.Common;

public class Result<T>
{
    public T? Data { get; set; }
    public List<string> ErrorMessages { get; set; }

}