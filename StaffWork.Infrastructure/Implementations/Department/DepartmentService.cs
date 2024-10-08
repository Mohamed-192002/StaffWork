using StaffWork.Core.Interfaces;
using StaffWork.Core.Models;

namespace StaffWork.Infrastructure.Implementations
{
	public class DepartmentService : ServicesBase<Department>
	{
		public DepartmentService(IGenericRepo<Department> genericRepo) : base(genericRepo)
		{
		}

		public override Task InsertAsync(Department model)
		{
            model.IsActive = true;
            model.DateCreated = DateTime.Now;
			return base.InsertAsync(model);
		}

		public override Task UpdateAsync(int Id, Department model)
		{
			model.DateModified = DateTime.Now;
			return base.UpdateAsync(Id, model);
		}
	}
}
