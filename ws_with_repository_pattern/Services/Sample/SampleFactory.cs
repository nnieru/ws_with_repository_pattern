using ws_with_repository_pattern.Common;
using ws_with_repository_pattern.Handler;
using ws_with_repository_pattern.Helper;
using ws_with_repository_pattern.Repository;

namespace ws_with_repository_pattern.Services;

public static class SampleFactory
{
    
    public static SampleHandler New(ISampleRepository repository, IValidator validator)
    {
        return new SampleHandler(repository, validator);
    }
}