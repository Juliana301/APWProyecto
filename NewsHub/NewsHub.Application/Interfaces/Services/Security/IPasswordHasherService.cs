using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Application.Interfaces.Services.Security
{
    public interface IPasswordHasherService
    {
        string HashPassword(string password);
        bool VerifyPassword(string hash, string password);
    }
}
