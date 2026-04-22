using System.Text.RegularExpressions;
using NewsHub.Domain.Interfaces.Repositories;

namespace NewsHub.Infrastructure.External.ApiSources.Helpers
{
    public class EnvironmentVariableResolver : IEnvironmentVariableResolver
    {
        private readonly ISecretRepository _secretRepository;

        public EnvironmentVariableResolver(ISecretRepository secretRepository)
        {
            _secretRepository = secretRepository;
        }

        public async Task<string?> ResolveAsync(string? value, int sourceId)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            var matches = Regex.Matches(value, @"\{\{(.*?)\}\}");

            foreach (Match match in matches)
            {
                var variableName = match.Groups[1].Value.Trim();

                var secret = await _secretRepository.FirstAsync(
                    s => s.Key == variableName && s.SourceId == sourceId);

                if (secret == null || string.IsNullOrWhiteSpace(secret.Value))
                {
                    throw new InvalidOperationException(
                        $"No se encontró el secret '{variableName}' para la fuente con Id {sourceId}.");
                }

                value = value.Replace(match.Value, secret.Value);
            }

            return value;
        }
    }
}