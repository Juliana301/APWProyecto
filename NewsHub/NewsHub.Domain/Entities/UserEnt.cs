using System;
using System.Collections.Generic;

namespace NewsHub.Domain.Entities
{
    public class UserEnt
    {
        public int Id { get; private set; }

        public string Email { get; private set; } = null!;
        public string UserName { get; private set; } = null!;
        public string PasswordHash { get; private set; } = null!;

        public string FirstName { get; private set; } = null!;
        public string LastName { get; private set; } = null!;

        public bool IsActive { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime? LastLoginAt { get; private set; }

        public ICollection<UserRoleEnt> UserRoles { get; private set; } = new List<UserRoleEnt>();
        public ICollection<PasswordResetTokenEnt> PasswordResetTokens { get; private set; } = new List<PasswordResetTokenEnt>();
        
        private UserEnt() { } // EF

        public UserEnt(string email, string userName, string passwordHash, string firstName, string lastName)
        {
            Email = email;
            UserName = userName;
            PasswordHash = passwordHash;
            FirstName = firstName;
            LastName = lastName;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public void AddRole(int roleId)
        {
            if (UserRoles.Any(ur => ur.RoleId == roleId))
                return;

            UserRoles.Add(new UserRoleEnt(this.Id, roleId));
        }

        public void ChangePassword(string newPasswordHash)
        {
            PasswordHash = newPasswordHash;
        }

        public void UpdateLastLogin()
        {
            LastLoginAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
        }
    }
}