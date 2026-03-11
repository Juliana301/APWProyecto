using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Application.DTOs.Authentication
{
    public class ResetPasswordTokenDto
    {
        public string Token { get; set; } = null!;
        public string Password { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
    }
}
