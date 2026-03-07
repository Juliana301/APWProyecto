using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Application.DTOs.Authentication
{
    public class LoginDto
    {
        public string UserName { get; init; } = null!;
        public string Password { get; init; } = null!;
    }
}
