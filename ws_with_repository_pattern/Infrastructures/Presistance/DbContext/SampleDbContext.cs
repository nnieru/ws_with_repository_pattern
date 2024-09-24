using Microsoft.EntityFrameworkCore;
using ws_with_repository_pattern.Domain.Model.Sample;

namespace ws_with_repository_pattern.Infrastructures.Presistance.DbContext;

public class SampleDbContext: Microsoft.EntityFrameworkCore.DbContext
{

    public SampleDbContext(DbContextOptions<SampleDbContext> options) 
        : base(options)
    {
    }

    public SampleDbContext()
    {
        
    }
    
    public DbSet<Sample> Samples { get; set; }
}