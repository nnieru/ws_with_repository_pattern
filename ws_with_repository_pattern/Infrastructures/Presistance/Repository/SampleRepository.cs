using ws_with_repository_pattern.Domain.Model.Sample;
using ws_with_repository_pattern.Infrastructures.Presistance.DbContext;

namespace ws_with_repository_pattern.Infrastructures.Presistance.Repository;

public interface ISampleRepository
{
    public IEnumerable<Sample> GetAllSample();
}

public class SampleRepository: BaseRepository<Sample>, ISampleRepository
{

    public SampleRepository(SampleDbContext context) : base(context)
    {
        
    }
    

    public IEnumerable<Sample> GetAllSample()
    {
        return GetAll();
    }
}



