using Newtonsoft.Json;

namespace ws_with_repository_pattern.Model.Dto;

public class SampleRequestDto
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
}