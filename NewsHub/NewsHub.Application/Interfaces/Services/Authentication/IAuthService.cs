using NewsHub.Application.Common;
using NewsHub.Application.DTOs.Authentication;
using NewsHub.Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Application.Interfaces.Services.Authentication
{
    public interface IAuthService
    {
        Task<Result<bool>> RegisterAsync(RegisterDto dto);
        Task<Result<UserDto>> LoginAsync(LoginDto dto);
        Task<Result<bool>> ForgotPasswordAsync(ForgotPasswordDto dto);
        Task<Result<bool>> ResetPasswordAsync(PasswordResetTokenDto dto);
    }
}
