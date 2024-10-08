using Microsoft.EntityFrameworkCore;
using StaffWork.Core.Data;
using StaffWork.Core.Interfaces;
using System.Linq.Expressions;

namespace StaffWork.Infrastructure.Implementations
{
    public class GenericRepo<T>(ApplicationDbContext context) : IGenericRepo<T> where T : class
    {
        private readonly ApplicationDbContext _context = context;


        public async Task<IEnumerable<T>> GetAllAsync()
              => await _context.Set<T>().AsNoTracking().ToListAsync();
        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public async Task AddRangeAsync(List<T> entity)
        {
            await _context.Set<T>().AddRangeAsync(entity);
            await _context.SaveChangesAsync();
        }
        public async Task Update(int id, T value)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity is not null)
            {
                _context.Entry(entity).CurrentValues.SetValues(value);
                _context.Update(entity);
                await _context.SaveChangesAsync();
            }
        }
        public async Task UpdateRangeAsync(List<T> value)
        {
            var entity = _context.Set<T>();
            entity.UpdateRange(value);
            await _context.SaveChangesAsync();

        }
        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        public Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> criteria = null, string[] includes = null, Expression<Func<T, object>> orderBy = null, string orderByDirection = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (criteria != null)
            {
                query = query.Where(criteria);
            }

            if (orderBy != null)
            {
                if (!string.IsNullOrEmpty(orderByDirection))
                {
                    query = orderByDirection.ToUpper() == "ASC" ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
                }
            }

            if (includes != null)
                foreach (var incluse in includes)
                    query = query.Include(incluse);



            return Task.FromResult(query.AsEnumerable());
        }

        public async Task<T?> GetAsync(Expression<Func<T, bool>> criteria, string[] includes = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
                foreach (var incluse in includes)
                    query = query.Include(incluse);

            return await query.SingleOrDefaultAsync(criteria);
        }

    }
}
