using Microsoft.EntityFrameworkCore.Storage;
using NewsHub.Application.Interfaces.Persistence;

namespace NewsHub.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // BEGIN TRANSACTION
        // ============================================================

        public async Task BeginTransactionAsync()
        {
            if (_transaction != null)
                return;

            _transaction =
                await _context
                    .Database
                    .BeginTransactionAsync();
        }

        // ============================================================
        // COMMIT
        // ============================================================

        public async Task CommitAsync()
        {
            try
            {
                await _context.SaveChangesAsync();

                if (_transaction != null)
                {
                    await _transaction.CommitAsync();

                    await _transaction.DisposeAsync();

                    _transaction = null;
                }
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
        }

        // ============================================================
        // ROLLBACK
        // ============================================================

        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();

                await _transaction.DisposeAsync();

                _transaction = null;
            }
        }

        // ============================================================
        // SAVE CHANGES (sin transacción explícita)
        // ============================================================

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}