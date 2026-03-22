using NewsHub.Domain.Entities;

namespace NewsHub.Domain.Interfaces.Repositories
{
    public interface ISourceRepository : IGenericRepository<SourceEnt>
    {
        Task<SourceEnt?> GetByUrlAsync(string url);
    }
}
