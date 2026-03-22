using Microsoft.EntityFrameworkCore;
using NewsHub.Domain.Entities;
using NewsHub.Domain.Interfaces.Repositories;
using NewsHub.Domain.Interfaces.Repositories.ErrorCatch;
using NewsHub.Infrastructure.Data;

namespace NewsHub.Infrastructure.Repositories
{
    public class SourceRepository : GenericRepository<SourceEnt>, ISourceRepository
    {
        public SourceRepository(ApplicationDbContext context, ILogErrorRepository logError) : base(context, logError)
        {
        }

        public async Task<SourceEnt?> GetByUrlAsync(string url)
        {
            return await FirstAsync(s => s.Url == url);
        }
    }
}
