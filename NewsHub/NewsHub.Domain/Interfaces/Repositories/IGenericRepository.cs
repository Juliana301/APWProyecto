using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace NewsHub.Domain.Interfaces.Repositories
{
    public interface IGenericRepository<TEntity>
        where TEntity : class
    {
        Task<TEntity?> GetByIdAsync(
            int id,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null,
            bool asNoTracking = true);

        Task<TEntity?> FirstAsync(
            Expression<Func<TEntity, bool>> predicate,
            bool asNoTracking = true,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null);

        Task<List<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> predicate,
            bool asNoTracking = true,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null);

        Task<List<TEntity>> GetAllAsync(
            bool asNoTracking = true,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null);

        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);

        Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null);

        Task<TEntity?> AddAsync(TEntity entity);

        Task<TEntity?> UpdateAsync(TEntity entity);

        Task<bool> DeleteAsync(int id);

        Task<(List<TEntity> Data, int Total)> GetPagedAsync(
            int page,
            int pageSize,
            bool asNoTracking = true,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null);
    }
}
