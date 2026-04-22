namespace NewsHub.Infrastructure.External.ApiSources.Helpers
{
    public interface IEnvironmentVariableResolver
    {
        Task<string?> ResolveAsync(string? value, int sourceId);
    }
}