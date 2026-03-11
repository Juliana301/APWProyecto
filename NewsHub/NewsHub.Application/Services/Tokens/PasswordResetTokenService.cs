using NewsHub.Application.Common;
using NewsHub.Application.DTOs.Authentication;
using NewsHub.Application.Interfaces.Tokens;
using NewsHub.Domain.Entities;
using NewsHub.Domain.Interfaces.Repositories.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Application.Services.Tokens
{
    public class PasswordResetTokenService : IPasswordResetTokenService
    {
        private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;

        public PasswordResetTokenService(
            IPasswordResetTokenRepository passwordResetTokenRepository
        )
        {
            _passwordResetTokenRepository = passwordResetTokenRepository;
        }

        public async Task<Result<ResetPasswordTokenDto>> CreateTokenAsync(int userId)
        {
            var tokenValue = Guid.NewGuid().ToString();

            var expiresAt = DateTime.UtcNow.AddMinutes(30);

            var token = new PasswordResetToken(userId, tokenValue, expiresAt);

            await _passwordResetTokenRepository.AddAsync(token);

            return Result<ResetPasswordTokenDto>.Ok(new ResetPasswordTokenDto
            {
                Token = tokenValue,
                ExpiresAt = expiresAt
            });
        }

        public async Task MarkTokenAsUsedAsync(PasswordResetToken token)
        {
            token.MarkAsUsed();
            await _passwordResetTokenRepository.UpdateAsync(token);
        }

        public async Task<Result<PasswordResetToken>> ValidateTokenAsync(string tokenValue)
        {
            var token = await _passwordResetTokenRepository.FirstAsync(
                t => t.Token == tokenValue
            );

            if (token == null)
                return Result<PasswordResetToken>.Fail("Token inválido.", TypeMessage.Error);

            if (!token.IsValid())
                return Result<PasswordResetToken>.Fail("El token ha expirado o ya fue usado.", TypeMessage.Warning);

            return Result<PasswordResetToken>.Ok(token);
        }
    }
}
