using System.Linq.Expressions;

namespace StaffWork.Core.Interfaces
{
	public interface IServicesBase<model> where model : class
    {
        Task InsertAsync(model model);
        Task InsertRangeAsync(List<model> model);
        Task UpdateAsync(int Id,model model);
        Task UpdateRangeAsync(List<model> model);
        Task DeleteAsync(int id);
        Task<IEnumerable<model>> GetAllAsync(Expression<Func<model, bool>> criteria = null, string[] includes = null, Expression<Func<model, object>> orderBy = null, string orderByDirection = null);
        Task<model> GetAsync(Expression<Func<model, bool>> criteria, string[] includes = null);
        Task SeenNotification(int Id, model model);
    }
}