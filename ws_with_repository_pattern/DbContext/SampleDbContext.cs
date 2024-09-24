using Microsoft.EntityFrameworkCore;
using ws_with_repository_pattern.Model.Dto;

namespace ws_with_repository_pattern.DbContext;

public class SampleDbContext: Microsoft.EntityFrameworkCore.DbContext
{

    public SampleDbContext(DbContextOptions<SampleDbContext> options) 
        : base(options)
    {
    }
    
    public DbSet<Sample> Samples { get; set; }
}