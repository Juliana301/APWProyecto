using NewsHub.Domain.Entities;
using NewsHub.Infrastructure.Repositories;

namespace NewsHub.Services.Services;

public class SourceService
{
    private readonly SourceRepository _sourceRepository;

    public SourceService(SourceRepository sourceRepository)
    {
        _sourceRepository = sourceRepository;
    }

    public async Task<List<Source>> GetAllSourcesAsync()
    {
        return await _sourceRepository.GetAllAsync();
    }

    public async Task<Source?> GetSourceByIdAsync(int id)
    {
        return await _sourceRepository.GetByIdAsync(id);
    }

    public async Task CreateSourceAsync(Source source)
    {
        await _sourceRepository.AddAsync(source);
    }

    public async Task UpdateSourceAsync(Source source)
    {
        await _sourceRepository.UpdateAsync(source);
    }

    public async Task DeleteSourceAsync(int id)
    {
        var source = await _sourceRepository.GetByIdAsync(id);

        if (source != null)
        {
            await _sourceRepository.DeleteAsync(source);
        }
    }
}