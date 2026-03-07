using Microsoft.EntityFrameworkCore;
using NewsHub.Domain.Interfaces.Repositories;
using NewsHub.Domain.Interfaces.Repositories.ErrorCatch;
using NewsHub.Infrastructure.Data;
using System.Linq.Expressions;

namespace NewsHub.Infrastructure.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;
        protected readonly ILogErrorRepository _logError;

        public GenericRepository(
            ApplicationDbContext context,
            ILogErrorRepository logError)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
            _logError = logError;
        }

        protected async Task Log(string method, Exception ex)
        {
            await _logError.AddLogErrorAsync($"{typeof(TEntity).Name}.{method}", ex);
        }

        // ============================================================
        // GET BY ID
        // ============================================================
        public virtual async Task<TEntity?> GetByIdAsync(
            int id,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null,
            bool asNoTracking = true)
        {
            try
            {
                var key = _context.Model
                    .FindEntityType(typeof(TEntity))!
                    .FindPrimaryKey()!
                    .Properties
                    .First()
                    .Name;

                IQueryable<TEntity> query = _dbSet;

                if (include != null)
                    query = include(query);

                if (asNoTracking)
                    query = query.AsNoTracking();

                return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, key) == id);
            }
            catch (Exception ex)
            {
                await Log(nameof(GetByIdAsync), ex);
                return null;
            }
        }

        // ============================================================
        // FIRST (primer resultado)
        // ============================================================
        public virtual async Task<TEntity?> FirstAsync(
            Expression<Func<TEntity, bool>> predicate,
            bool asNoTracking = true,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null)
        {
            try
            {
                IQueryable<TEntity> query = _dbSet;

                if (include != null)
                    query = include(query);

                if (asNoTracking)
                    query = query.AsNoTracking();

                return await query.FirstOrDefaultAsync(predicate);
            }
            catch (Exception ex)
            {
                await Log(nameof(FirstAsync), ex);
                return null;
            }
        }

        // ============================================================
        // FIND (Where)
        // ============================================================
        public virtual async Task<List<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> predicate,
            bool asNoTracking = true,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null)
        {
            try
            {
                IQueryable<TEntity> query = _dbSet.Where(predicate);

                if (include != null)
                    query = include(query);

                if (asNoTracking)
                    query = query.AsNoTracking();

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                await Log(nameof(FindAsync), ex);
                return new List<TEntity>();
            }
        }

        // ============================================================
        // GET ALL
        // ============================================================
        public virtual async Task<List<TEntity>> GetAllAsync(
            bool asNoTracking = true,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null)
        {
            try
            {
                IQueryable<TEntity> query = _dbSet;

                if (include != null)
                    query = include(query);

                if (asNoTracking)
                    query = query.AsNoTracking();

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                await Log(nameof(GetAllAsync), ex);
                return new List<TEntity>();
            }
        }

        // ============================================================
        // EXISTS
        // ============================================================
        public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
        {
            try
            {
                return await _dbSet.AnyAsync(predicate);
            }
            catch (Exception ex)
            {
                await Log(nameof(ExistsAsync), ex);
                return false;
            }
        }

        // ============================================================
        // COUNT
        // ============================================================
        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null)
        {
            try
            {
                IQueryable<TEntity> query = _dbSet;

                if (predicate != null)
                    query = query.Where(predicate);

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                await Log(nameof(CountAsync), ex);
                return 0;
            }
        }

        // ============================================================
        // ADD
        // ============================================================
        public virtual async Task<TEntity?> AddAsync(TEntity entity)
        {
            try
            {
                await _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync();

                return entity;
            }
            catch (Exception ex)
            {
                await Log(nameof(AddAsync), ex);
                return null;
            }
        }

        // ============================================================
        // UPDATE
        // ============================================================
        public virtual async Task<TEntity?> UpdateAsync(TEntity entity)
        {
            try
            {
                _dbSet.Update(entity);

                var saved = await _context.SaveChangesAsync() > 0;

                if (!saved)
                    return null;

                return entity;
            }
            catch (Exception ex)
            {
                await Log(nameof(UpdateAsync), ex);
                return null;
            }
        }

        // ============================================================
        // DELETE
        // ============================================================
        public virtual async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var entity = await GetByIdAsync(id, asNoTracking: false);

                if (entity == null)
                    return false;

                _dbSet.Remove(entity);

                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                await Log(nameof(DeleteAsync), ex);
                return false;
            }
        }

        // ============================================================
        // PAGINACIÓN
        // ============================================================
        public virtual async Task<(List<TEntity> Data, int Total)> GetPagedAsync(
            int page,
            int pageSize,
            bool asNoTracking = true,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null)
        {
            try
            {
                IQueryable<TEntity> query = _dbSet;

                if (include != null)
                    query = include(query);

                if (asNoTracking)
                    query = query.AsNoTracking();

                var total = await query.CountAsync();

                var data = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return (data, total);
            }
            catch (Exception ex)
            {
                await Log(nameof(GetPagedAsync), ex);
                return (new List<TEntity>(), 0);
            }
        }
    }
}