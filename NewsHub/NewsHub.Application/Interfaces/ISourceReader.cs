using NewsHub.Application.Common.Models;
using NewsHub.Domain.Entities;
using NewsHub.Domain.Enums;

namespace NewsHub.Application.Interfaces
{
    public interface ISourceReader
    {
        SourceType Type { get; }
        Task<List<SourceItem>> ReadAsync(SourceEnt source);
    }
}
