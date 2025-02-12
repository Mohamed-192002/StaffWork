using StaffWork.Core.Interfaces;
using StaffWork.Core.Models;

namespace StaffWork.Infrastructure.Implementations
{
    public class VacationTypeService : ServicesBase<VacationType>
    {
        public VacationTypeService(IGenericRepo<VacationType> genericRepo) : base(genericRepo)
        {
        }

        public override Task InsertAsync(VacationType model)
        {
            model.IsActive = true;
            model.DateCreated = DateTime.Now;
            return base.InsertAsync(model);
        }

        public override Task UpdateAsync(int Id, VacationType model)
        {
            model.DateModified = DateTime.Now;
            return base.UpdateAsync(Id, model);
        }
    }
}
