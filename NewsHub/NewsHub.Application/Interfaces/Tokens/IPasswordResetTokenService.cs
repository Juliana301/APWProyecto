using NewsHub.Application.Common;
using NewsHub.Application.DTOs.Authentication;
using NewsHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Application.Interfaces.Tokens
{
    public interface IPasswordResetTokenService
    {
        Task<Result<PasswordResetTokenDto>> CreateTokenAsync(int userId);
        Task<Result<PasswordResetTokenEnt>> ValidateTokenAsync(string tokenValue);
        Task MarkTokenAsUsedAsync(PasswordResetTokenEnt token);
    }
}
