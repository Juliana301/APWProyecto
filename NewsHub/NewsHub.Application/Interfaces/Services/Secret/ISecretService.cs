using System;
using NewsHub.Application.Common;
using NewsHub.Application.DTOs.Secret;
using NewsHub.Domain.Entities;

namespace NewsHub.Application.Interfaces.Services.Secret
{
    public interface ISecretService
    {
        Task<Result<bool>> CreateAsync(CreateSecretDto dto);
        Task<Result<List<SecretEnt>>> GetAllAsync();
        Task<Result<bool>> DeleteAsync(int id);
    }
}