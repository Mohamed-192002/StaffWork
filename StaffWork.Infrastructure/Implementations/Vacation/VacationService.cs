using StaffWork.Core.Interfaces;
using StaffWork.Core.Models;

namespace StaffWork.Infrastructure.Implementations
{
    public class VacationsService : ServicesBase<Vacation>
    {
        public VacationsService(IGenericRepo<Vacation> genericRepo) : base(genericRepo)
        {
        }

        public override Task InsertAsync(Vacation model)
        {
            model.IsActive = true;
            model.DateCreated = DateTime.Now;
            return base.InsertAsync(model);
        }

        public override Task UpdateAsync(int Id, Vacation model)
        {
            model.DateModified = DateTime.Now;
            return base.UpdateAsync(Id, model);
        }
    }
}
