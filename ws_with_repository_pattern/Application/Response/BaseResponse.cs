using System.Net;

namespace ws_with_repository_pattern.Response;

public class BaseResponse<T>
{
    public HttpStatusCode StatusCode { get; set; }
    public T? data { get; set; }
    public string message { get; set; }
}