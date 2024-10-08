using StaffWork.Core.Interfaces;
using System.Linq.Expressions;

namespace StaffWork.Infrastructure.Implementations
{
	public abstract class ServicesBase<T> : IServicesBase<T> where T : class
	{
		public ServicesBase(IGenericRepo<T> genericRepo)
		{
			GenericRepo = genericRepo;
		}
		public IGenericRepo<T> GenericRepo { get; }

		public virtual async Task DeleteAsync(int id)
		{
			await GenericRepo.DeleteAsync(id);
		}

		public virtual async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> criteria = null, string[] includes = null, Expression<Func<T, object>> orderBy = null, string orderByDirection = null)
		{
			return await GenericRepo.GetAllAsync(criteria, includes, orderBy, orderByDirection);
		}

		public virtual async Task<T> GetAsync(Expression<Func<T, bool>> criteria, string[] includes = null)
		{
			return await GenericRepo.GetAsync(criteria, includes);
		}



		public virtual async Task InsertAsync(T model)
		{
			await GenericRepo.AddAsync(model);
		}

		public virtual async Task InsertRangeAsync(List<T> model)
		{
			await GenericRepo.AddRangeAsync(model);
		}

		public virtual async Task SeenNotification(int Id, T model)
		{
			await GenericRepo.Update(Id, model);
		}

		public virtual async Task UpdateAsync(int Id, T model)
		{
			await GenericRepo.Update(Id, model);
		}

		public virtual async Task UpdateRangeAsync(List<T> model)
		{
			await GenericRepo.UpdateRangeAsync(model);
		}
	}
}
