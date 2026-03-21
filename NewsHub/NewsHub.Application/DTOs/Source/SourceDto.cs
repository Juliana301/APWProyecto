using NewsHub.Domain.Enums;

namespace NewsHub.Application.DTOs.Source
{
    public class SourceDto
    {
        public int Id { get; init; }
        public string Url { get; init; } = null!;
        public string Name { get; init; } = null!;
        public string? Description { get; init; }
        public SourceType ComponentType { get; init; }
        public bool RequiresSecret { get; init; }
    }
}
