using System.Linq.Expressions;


namespace StaffWork.Core.Interfaces
{
    public interface IGenericRepo<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> criteria = null, string[] includes = null, Expression<Func<T, object>> orderBy = null, string orderByDirection = null);
        Task<T> GetAsync(Expression<Func<T, bool>> criteria, string[] includes = null);
        Task AddAsync(T entity);
        Task AddRangeAsync(List<T> entity);
        Task Update(int id, T value);
        Task UpdateRangeAsync(List<T> value);
        Task DeleteAsync(int id);
    }
}
