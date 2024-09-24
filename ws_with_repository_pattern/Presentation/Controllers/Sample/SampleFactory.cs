using ws_with_repository_pattern.Application.Handler;
using ws_with_repository_pattern.Domain.Common;
using ws_with_repository_pattern.Infrastructures.Presistance.Repository;
using ws_with_repository_pattern.Infrastructures.Presistance.UnitOfWorks;

namespace ws_with_repository_pattern.Presentation.Controllers.Sample;

public static class SampleFactory
{
    
    public static SampleHandler New(SampleUnitOfWork unitOfWork, IValidator validator)
    {
        return new SampleHandler(unitOfWork, validator);
    }
}