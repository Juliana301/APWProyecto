using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Application.DTOs.Authentication
{
    public class RegisterDto
    {
        public string Email { get; init; } = null!;
        public string UserName { get; init; } = null!;
        public string Password { get; init; } = null!;
        public string FirstName { get; init; } = null!;
        public string LastName { get; init; } = null!;
    }
}