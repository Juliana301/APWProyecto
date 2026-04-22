using Microsoft.AspNetCore.Mvc.Rendering;
using NewsHub.Domain.Enums;

namespace NewsHub.Web.ViewModels.Settings
{
    public class SettingsViewModel
    {
        public CreateSecretViewModel NewSecret { get; set; } = new();
        public List<UsersViewModel> Users { get; set; } = new();
        public List<SecretViewModel> Secrets { get; set; } = new();
        public List<SelectListItem> SourceItems { get; set; } = new();
    }

    public class UsersViewModel
    {
        public int Id { get; init; }
        public string Email { get; init; } = null!;
        public string UserName { get; init; } = null!;
        public string FirstName { get; init; } = null!;
        public string LastName { get; init; } = null!;
        public bool IsActive { get; init; }
        public RolesEnums Role { get; init; }
    }

    public class UpdateUserViewModel
    {
        public int UserId { get; set; }
        public RolesEnums Role { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateSecretViewModel
    {
        public string Key { get; set; } = null!;
        public string Value { get; set; } = null!;
        public bool IsEncrypted { get; set; }
        public int SourceId { get; set; }
    }

    public class SecretViewModel
    {
        public int Id { get; init; }
        public string Key { get; init; } = null!;
        public string Value { get; init; } = null!;
        public bool IsEncrypted { get; init; }
        public string? SourceName { get; init; }
    }

    public class DeleteSecretViewModel
    {
        public int Id { get; set; }
    }
}