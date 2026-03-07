using NewsHub.Domain.Entities;
using NewsHub.Infrastructure.Data;

namespace NewsHub.Infrastructure.Repositories;

public class SourceRepository : RepositoryBase<Source>
{
    public SourceRepository(ApplicationDbContext context) : base(context)
    {
    }
}