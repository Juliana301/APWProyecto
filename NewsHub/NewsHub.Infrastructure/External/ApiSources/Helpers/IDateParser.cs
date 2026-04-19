namespace NewsHub.Infrastructure.External.ApiSources.Helpers
{
    public interface IDateParser
    {
        DateTime Parse(string? value);
    }
}