using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Domain.Entities
{
    public class PasswordResetTokenEnt
    {
        public int Id { get; private set; }

        public int UserId { get; private set; }

        public string Token { get; private set; } = null!;

        public DateTime ExpiresAt { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public bool Used { get; private set; }

        public UserEnt User { get; private set; } = null!;

        private PasswordResetTokenEnt() { } // EF

        public PasswordResetTokenEnt(int userId, string token, DateTime expiresAt)
        {
            UserId = userId;
            Token = token;
            ExpiresAt = expiresAt;
            Used = false;
            CreatedAt = DateTime.UtcNow;
        }

        public bool IsValid()
        {
            return !Used && DateTime.UtcNow <= ExpiresAt;
        }

        public void MarkAsUsed()
        {
            Used = true;
        }
    }
}
