using NewsHub.Domain.Entities;
using NewsHub.Infrastructure.Data;

namespace NewsHub.Infrastructure.Repositories;

public class SecretRepository : RepositoryBase<Secret>
{
    public SecretRepository(ApplicationDbContext context) : base(context)
    {
    }
}