using NewsHub.Domain.Entities;
using NewsHub.Domain.Interfaces.Repositories;
using NewsHub.Domain.Interfaces.Repositories.ErrorCatch;
using NewsHub.Domain.Interfaces.Repositories.Tokens;
using NewsHub.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Infrastructure.Repositories.Tokens
{
    public class PasswordResetTokenRepository : GenericRepository<PasswordResetToken>, IPasswordResetTokenRepository
    {
        public PasswordResetTokenRepository(
            ApplicationDbContext context, 
            ILogErrorRepository logError
            ) : base(context, logError)
        {
        }
    }
}
