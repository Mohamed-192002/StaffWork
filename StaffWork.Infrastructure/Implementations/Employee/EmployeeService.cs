using StaffWork.Core.Interfaces;
using StaffWork.Core.Models;

namespace StaffWork.Infrastructure.Implementations
{
    public class EmployeeService : ServicesBase<Employee>
    {
        public EmployeeService(IGenericRepo<Employee> genericRepo) : base(genericRepo)
        {
        }

        public override Task InsertAsync(Employee model)
        {
            model.IsActive = true;
            model.DateCreated = DateTime.Now;
            return base.InsertAsync(model);
        }

        public override Task UpdateAsync(int Id, Employee model)
        {
            model.DateModified = DateTime.Now;
            return base.UpdateAsync(Id, model);
        }
    }
}
