namespace NewsHub.Infrastructure.External.ApiSources.Helpers
{
    public interface IEnvironmentVariableResolver
    {
        string? Resolve(string? value);
    }
}