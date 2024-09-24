using Binus.WS.Pattern.Output;

namespace ws_with_repository_pattern.Domain.Output;

public class SampleOutput: OutputBase
{
    public object Data { get; set; }

    public SampleOutput()
    {
        this.Data = new object();
    }
}