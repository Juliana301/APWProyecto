using System;
using NewsHub.Domain.Entities;

namespace NewsHub.Domain.Interfaces.Repositories
{
    public interface ISecretRepository : IGenericRepository<SecretEnt>
    {
        Task<List<SecretEnt>> GetAllWithSourceAsync();
    }
}
