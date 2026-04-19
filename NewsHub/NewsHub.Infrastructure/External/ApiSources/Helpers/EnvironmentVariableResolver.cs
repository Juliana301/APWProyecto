using System.Text.RegularExpressions;

namespace NewsHub.Infrastructure.External.ApiSources.Helpers
{
    public class EnvironmentVariableResolver : IEnvironmentVariableResolver
    {
        public string? Resolve(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            var matches = Regex.Matches(value, @"\{\{(.*?)\}\}");

            foreach (Match match in matches)
            {
                var variableName = match.Groups[1].Value;

                var environmentValue = Environment.GetEnvironmentVariable(variableName);

                if (!string.IsNullOrWhiteSpace(environmentValue))
                {
                    value = value.Replace(match.Value, environmentValue);
                }
            }

            return value;
        }
    }
}