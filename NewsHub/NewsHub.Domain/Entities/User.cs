using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Domain.Entities
{
    public class User
    {
        public int Id { get; private set; }

        public string Email { get; private set; } = null!;
        public string PasswordHash { get; private set; } = null!;

        public string FirstName { get; private set; } = null!;
        public string LastName { get; private set; } = null!;

        public bool IsActive { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime? LastLoginAt { get; private set; }

        public ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();

        private User() { } // EF

        public User(string email, string passwordHash, string firstName, string lastName)
        {
            Email = email;
            PasswordHash = passwordHash;
            FirstName = firstName;
            LastName = lastName;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
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
