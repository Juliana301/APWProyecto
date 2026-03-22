using NewsHub.Application.Common.Models;
using NewsHub.Domain.Entities;
using NewsHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Application.Interfaces
{
    public interface ISourceReader
    {
        SourceType Type { get; }
        Task<List<SourceItem>> ReadAsync(SourceEnt source);
    }
}
