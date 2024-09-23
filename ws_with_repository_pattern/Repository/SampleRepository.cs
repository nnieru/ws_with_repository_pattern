using ws_with_repository_pattern.Model.Dto;

namespace ws_with_repository_pattern.Repository;

public interface ISampleRepository
{
    Sample? Get(SampleRequestDto request);
}

public class SampleRepository: ISampleRepository
{
    public Sample? Get(SampleRequestDto request)
    {
        return new Sample
        {
            Id = "0x00000000",
            Name = request.Name,
            Address = "Jl. xxxxx xxxx",
            Phone = "+62 xxx xxx xxx"
        };
    }
}