using NewsHub.Domain.Entities;
using NewsHub.Infrastructure.Data;

namespace NewsHub.Infrastructure.Repositories;

public class SourceItemRepository : RepositoryBase<SourceItem>
{
    public SourceItemRepository(ApplicationDbContext context) : base(context)
    {
    }
}