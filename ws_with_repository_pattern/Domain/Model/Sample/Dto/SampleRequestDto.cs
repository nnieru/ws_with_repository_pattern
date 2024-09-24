using Newtonsoft.Json;

namespace ws_with_repository_pattern.Domain.Model.Sample.Dto;

public class SampleRequestDto
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
}