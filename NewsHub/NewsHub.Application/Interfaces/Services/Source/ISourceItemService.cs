using NewsHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Application.Interfaces.Services.Source
{
    public interface ISourceItemService
    {
        Task<bool> SaveAsync(
            int sourceId,
            string json);

        Task<List<SourceItemEnt>> GetSavedAsync();

        Task<int> ImportAsync(
            int sourceId,
            List<string> jsonItems);
    }
}
