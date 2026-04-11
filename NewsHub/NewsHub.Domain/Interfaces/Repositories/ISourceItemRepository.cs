using NewsHub.Domain.Entities;

namespace NewsHub.Domain.Interfaces.Repositories
{
    public interface ISourceItemRepository : IGenericRepository<SourceItemEnt>
    {
        Task<bool> ExistsByJsonAsync(string json);

        Task<List<SourceItemEnt>> GetLatestAsync(
            int limit);

        Task<SourceItemEnt?> GetByJsonAsync(
            string json);
    }
}