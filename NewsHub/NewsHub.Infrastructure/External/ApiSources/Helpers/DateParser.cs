namespace NewsHub.Infrastructure.External.ApiSources.Helpers
{
    public class DateParser : IDateParser
    {
        public DateTime Parse(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return DateTime.UtcNow;

            var text = value.Trim();

            // Unix timestamp
            if (long.TryParse(text, out var unix))
            {
                if (text.Length >= 13)
                    return DateTimeOffset.FromUnixTimeMilliseconds(unix).UtcDateTime;

                return DateTimeOffset.FromUnixTimeSeconds(unix).UtcDateTime;
            }

            // ISO / Offset
            if (DateTimeOffset.TryParse(text, out var offset))
                return offset.UtcDateTime;

            // Date normal
            if (DateTime.TryParse(text, out var date))
                return DateTime.SpecifyKind(date, DateTimeKind.Utc);

            return DateTime.UtcNow;
        }
    }
}