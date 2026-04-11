using Microsoft.EntityFrameworkCore;
using NewsHub.Domain.Entities;
using NewsHub.Domain.Interfaces.Repositories;
using NewsHub.Domain.Interfaces.Repositories.ErrorCatch;
using NewsHub.Infrastructure.Data;

namespace NewsHub.Infrastructure.Repositories
{
    public class SourceItemRepository : GenericRepository<SourceItemEnt>, ISourceItemRepository
    {

        public SourceItemRepository(ApplicationDbContext context, ILogErrorRepository logError) : base(context, logError)
        {
        }

        public async Task<bool> ExistsByJsonAsync(string json)
        {
            try
            {
                return await _dbSet
                    .AnyAsync(x => x.Json == json);
            }
            catch (Exception ex)
            {
                await Log(nameof(ExistsByJsonAsync), ex);
                return false;
            }
        }

        public async Task<List<SourceItemEnt>> GetLatestAsync(
            int limit)
        {
            try
            {
                return await _dbSet
                    .OrderByDescending(x => x.CreatedAt)
                    .Take(limit)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                await Log(nameof(GetLatestAsync), ex);
                return new List<SourceItemEnt>();
            }
        }

        public async Task<SourceItemEnt?> GetByJsonAsync(
            string json)
        {
            try
            {
                return await _dbSet
                    .FirstOrDefaultAsync(
                        x => x.Json == json);
            }
            catch (Exception ex)
            {
                await Log(nameof(GetByJsonAsync), ex);
                return null;
            }
        }
    }
}