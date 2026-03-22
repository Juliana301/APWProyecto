using NewsHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Domain.Interfaces.Repositories.Tokens
{
    public interface IPasswordResetTokenRepository : IGenericRepository<PasswordResetTokenEnt>
    {

    }
}
