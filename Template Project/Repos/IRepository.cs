using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace Template_Project.Repos
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAsync(
          Expression<Func<T, bool>>? expression = null,
          Expression<Func<T, object>>[]? includes = null,
          bool tracked = true,
          CancellationToken cancellationToken = default);

        Task<T> GetOneAsync(
            Expression<Func<T, bool>>? expression = null,
            Expression<Func<T, object>>[]? includes = null,
            bool tracked = true,
            CancellationToken cancellationToken = default);

        Task<EntityEntry<T>> AddAsync(T entity, CancellationToken cancellationToken = default);

        void Update(T entity);

        void Delete(T entity);

        Task CommitAsync(CancellationToken cancellationToken = default);

    }
}
