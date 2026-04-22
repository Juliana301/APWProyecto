using Microsoft.EntityFrameworkCore;
using NewsHub.Domain.Entities;
using NewsHub.Domain.Interfaces.Repositories;
using NewsHub.Domain.Interfaces.Repositories.ErrorCatch;
using NewsHub.Infrastructure.Data;

namespace NewsHub.Infrastructure.Repositories
{
    public class SecretRepository : GenericRepository<SecretEnt>, ISecretRepository
    {
        public SecretRepository(
            ApplicationDbContext context,
            ILogErrorRepository logError) : base(context, logError)
        {
        }

        public async Task<List<SecretEnt>> GetAllWithSourceAsync()
        {
            return await GetAllAsync(
                include: query => query.Include(s => s.Source)
            );
        }
    }
}