namespace NewsHub.Application.DTOs.Secret
{
    public class CreateSecretDto
    {
        public string Key { get; set; } = null!;
        public string Value { get; set; } = null!;
        public bool IsEncrypted { get; set; }
        public int SourceId { get; set; }
    }
}