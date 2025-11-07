using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Template_Project.DataAccess;

namespace Template_Project.Repos
{
    public class Repository<T> where T : class
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        private readonly DbSet<T> _dbSet;

        public Repository()
        {
            _dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAsync(
    Expression<Func<T, bool>>? expression = null,
    Expression<Func<T, object>>[]? includes = null,
    bool tracked = true,
    CancellationToken cancellationToken = default)
        {
            var entities = _dbSet.AsQueryable();
           
            if (expression is not null)
            {
                entities = entities.Where(expression);
            }
           
            if (includes is not null)
            {
                foreach (var include in includes)
                {
                    entities = entities.Include(include);
                }
            }
         
            if (!tracked)
            {
                entities = entities.AsNoTracking();
            }
      
            return await entities.ToListAsync(cancellationToken);
        }


        //public async Task<T> GetOneAsync(
        //    Expression<Func<T, bool>>? expression = null,
        //    Expression<Func<T, object>>[]? includes = null,
        //    bool tracked = true,
        //    CancellationToken cancellationToken = default)
        //{
        //    var entity = (await GetAsync(expression,includes,tracked,cancellationToken)).FirstOrDefault();
        //    return entity;
        //}

        public async Task<T> GetOneAsync(
    Expression<Func<T, bool>>? expression = null,
    Expression<Func<T, object>>[]? includes = null,
    bool tracked = true,
    CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _dbSet;

            if (expression is not null)
            {
                query = query.Where(expression);
            }

            if (includes is not null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            if (!tracked)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(cancellationToken);
        }


        public async Task<EntityEntry<T>> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
          var result= await _context.AddAsync(entity, cancellationToken);
            return result;
        }

        public void Update(T entity)
        {
            _context.Update(entity);
        }

        public void Delete(T entity)
        {
            _context.Remove(entity);
        }

        public async Task CommitAsync(CancellationToken cancellationToken=default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
