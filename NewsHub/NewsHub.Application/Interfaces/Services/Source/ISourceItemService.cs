using NewsHub.Application.Common;
using NewsHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Application.Interfaces.Services.Source
{
    public interface ISourceItemService
    {
        Task<Result<bool>> SaveAsync(
            int sourceId,
            string json);

        Task<Result<List<SourceItemEnt>>> GetSavedAsync();

        Task<int> ImportAsync(
            int sourceId,
            List<string> jsonItems);
    }
}
