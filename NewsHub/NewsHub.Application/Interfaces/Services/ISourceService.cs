using NewsHub.Application.Common;
using NewsHub.Application.Common.Models;
using NewsHub.Application.DTOs.Source;

namespace NewsHub.Application.Interfaces.Services
{
    public interface ISourceService
    {
        Task<Result<SourceDto>> GetByIdAsync(int id);
        Task<Result<List<SourceDto>>> GetAllAsync();
        Task<Result<SourceDto>> CreateAsync(SourceDto dto);
        Task<Result<SourceDto>> UpdateAsync(int id, SourceDto dto);
        Task<Result<bool>> DeleteAsync(int id);

        Task<Result<List<SourceItem>>> ReadAllFeedsAsync();
    }
}
