
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace ws_with_repository_pattern.Domain.DbContext;

public class KazutoDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public KazutoDbContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Use reflection to add DbSet properties based on the models in the specified folder
        // var assembly = Assembly.GetExecutingAssembly();
        // var assembly = Assembly.Load(new AssemblyName("ws_with_repository_pattern.Domain"));
        // var entityTypes = assembly.GetTypes()
        //     .Where(type => type.Namespace == "ws_with_repository_pattern.Domain.Entity" && type.IsClass);
        // foreach (var entityType in entityTypes)
        // {
        //     modelBuilder.Entity(entityType);
        // }
        
        string entityNamespace = "ws_with_repository_pattern.Domain.Entity";

        var targetAssembly = Assembly.Load("ws_with_repository_pattern");

        var entityTypes = targetAssembly.GetTypes().
            Where(t => t.Namespace == entityNamespace & t.IsClass && !t.IsAbstract);

        foreach (var type in entityTypes)
        {
            modelBuilder.Entity(type);
        }
    }
}
