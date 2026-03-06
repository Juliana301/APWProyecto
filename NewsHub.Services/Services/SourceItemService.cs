using NewsHub.Domain.Entities;
using NewsHub.Infrastructure.Repositories;

namespace NewsHub.Services.Services;

public class SourceItemService
{
    private readonly SourceItemRepository _sourceItemRepository;

    public SourceItemService(SourceItemRepository sourceItemRepository)
    {
        _sourceItemRepository = sourceItemRepository;
    }

    public async Task<List<SourceItem>> GetAllAsync()
    {
        return await _sourceItemRepository.GetAllAsync();
    }

    public async Task<SourceItem?> GetByIdAsync(int id)
    {
        return await _sourceItemRepository.GetByIdAsync(id);
    }

    public async Task CreateAsync(SourceItem item)
    {
        await _sourceItemRepository.AddAsync(item);
    }

    public async Task DeleteAsync(int id)
    {
        var item = await _sourceItemRepository.GetByIdAsync(id);

        if (item != null)
        {
            await _sourceItemRepository.DeleteAsync(item);
        }
    }
}