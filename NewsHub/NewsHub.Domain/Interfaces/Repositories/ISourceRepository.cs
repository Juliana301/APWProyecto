using NewsHub.Domain.Entities;

namespace NewsHub.Domain.Interfaces.Repositories
{
    public interface ISourceRepository : IGenericRepository<Source>
    {
        Task<Source?> GetByUrlAsync(string url);
    }
}
