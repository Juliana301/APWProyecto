using NewsHub.Application.Common;
using NewsHub.Application.DTOs.Authentication;
using NewsHub.Application.Interfaces.Persistence;
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

        public async Task<Result<PasswordResetTokenDto>> CreateTokenAsync(int userId)
        {
            var tokenValue =
                Guid.NewGuid().ToString("N");

            var expiresAt =
                DateTime.UtcNow.AddMinutes(30);

            var token =
                new PasswordResetTokenEnt(
                    userId,
                    tokenValue,
                    expiresAt
                );

            await _passwordResetTokenRepository.AddAsync(token);

            return Result<PasswordResetTokenDto>.Ok(
                new PasswordResetTokenDto
                {
                    Token = tokenValue,
                    ExpiresAt = expiresAt
                }
            );
        }

        public async Task MarkTokenAsUsedAsync(
            PasswordResetTokenEnt token
        )
        {
            token.MarkAsUsed();

            await _passwordResetTokenRepository.UpdateAsync(token);
        }

        public async Task<Result<PasswordResetTokenEnt>> ValidateTokenAsync(
            string tokenValue
        )
        {
            var token =
                await _passwordResetTokenRepository.FirstAsync(
                    t => t.Token == tokenValue
                );

            if (token == null)
                return Result<PasswordResetTokenEnt>.Fail(
                    "Token inválido.",
                    TypeMessage.Error
                );

            if (!token.IsValid())
                return Result<PasswordResetTokenEnt>.Fail(
                    "El token ha expirado o ya fue usado.",
                    TypeMessage.Warning
                );

            return Result<PasswordResetTokenEnt>.Ok(token);
        }
    }
}