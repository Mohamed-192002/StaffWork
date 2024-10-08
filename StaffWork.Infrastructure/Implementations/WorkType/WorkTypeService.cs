using StaffWork.Core.Interfaces;
using StaffWork.Core.Models;

namespace StaffWork.Infrastructure.Implementations
{
	public class WorkTypeService : ServicesBase<WorkType>
	{
		public WorkTypeService(IGenericRepo<WorkType> genericRepo) : base(genericRepo)
		{
		}

		public override Task InsertAsync(WorkType model)
		{
            model.IsActive = true;
            model.DateCreated = DateTime.Now;
			return base.InsertAsync(model);
		}

		public override Task UpdateAsync(int Id, WorkType model)
		{
			model.DateModified = DateTime.Now;
			return base.UpdateAsync(Id, model);
		}
	}
}
