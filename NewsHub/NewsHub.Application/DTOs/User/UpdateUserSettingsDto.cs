using System;
using NewsHub.Domain.Enums;

namespace NewsHub.Application.DTOs.User
{
    public class UpdateUserSettingsDto
    {
        public int UserId { get; set; }
        public RolesEnums Role { get; set; }
        public bool IsActive { get; set; }
    }
}