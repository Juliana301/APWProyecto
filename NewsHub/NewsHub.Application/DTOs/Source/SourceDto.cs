using NewsHub.Domain.Enums;

namespace NewsHub.Application.DTOs.Source
{
    public class SourceDto
    {
        public int Id { get; set; }
        public string Url { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public SourceType ComponentType { get; set; }
        public bool RequiresSecret { get; set; }

        public string? ApiConfigJson { get; set; }
    }
}
