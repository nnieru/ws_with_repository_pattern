using Microsoft.EntityFrameworkCore;
using ws_with_repository_pattern.DbContext;
using ws_with_repository_pattern.Model.Dto;

namespace ws_with_repository_pattern.Repository;

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



