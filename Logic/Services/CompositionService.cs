using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logic.Services;

public class CompositionService(IServiceScopeFactory scopeFactory)
{
    public void CreateComposition(CompositionCreationRequest request)
    {
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LgDbContext>();

        var composition = new Composition
        {
            Name = request.Name,
            Description = request.Description
        };

        request.Roles.ForEach(roleString =>
        {
            var role = dbContext.Roles.FirstOrDefault(r => r.Name == roleString);
            if (role is null) throw new KeyNotFoundException($"Role {roleString} not found");
            composition.Roles.Add(role);
        });

        dbContext.Compositions.Add(composition);
        dbContext.SaveChanges();
    }

    public CompositionGetResponse GetComposition(int id)
    {
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LgDbContext>();

        var composition = dbContext.Compositions.Include(composition => composition.Roles).FirstOrDefault(c => c.Id == id);
        return composition is null ?
            throw new KeyNotFoundException($"Composition {id} not found") :
            new CompositionGetResponse(composition.Id, composition.Name, composition.Description, composition.Roles.Select(r => r.Name).ToList());
    }
    
    public List<CompositionGetResponse> GetCompositions()
    {
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LgDbContext>();
        
        var compositions = dbContext.Compositions.Include(group => group.Roles).ToList();
        return compositions
            .Select(composition => new CompositionGetResponse(composition.Id, composition.Name, composition.Description, composition.Roles.Select(r => r.Name).ToList()))
            .ToList();
    }
}

public record CompositionGetResponse(int Id, string Name, string Description, List<string> Roles);

public record CompositionCreationRequest(string Name, string Description, List<string> Roles);