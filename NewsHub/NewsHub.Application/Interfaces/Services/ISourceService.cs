using NewsHub.Application.DTOs.Source;
using NewsHub.Application.Common;

namespace NewsHub.Application.Interfaces.Services
{
    public interface ISourceService
    {
        Task<Result<SourceDto>> GetByIdAsync(int id);
        Task<Result<List<SourceDto>>> GetAllAsync();
        Task<Result<SourceDto>> CreateAsync(SourceDto dto);
        Task<Result<SourceDto>> UpdateAsync(int id, SourceDto dto);
        Task<Result<bool>> DeleteAsync(int id);
    }
}
