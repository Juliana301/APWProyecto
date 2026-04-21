using System;
using NewsHub.Domain.Enums;

namespace NewsHub.Web.ViewModels.Settings;

public class SettingsViewModel
{
    public List<UsersViewModel> Users {get; set;} = new();
}

public class UsersViewModel
{
    public int Id { get; init; }
    public string Email { get; init; } = null!;
    public string UserName { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public bool IsActive { get; init; } = false;


    public RolesEnums Role { get; init; }
}
