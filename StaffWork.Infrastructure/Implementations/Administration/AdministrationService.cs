using StaffWork.Core.Interfaces;
using StaffWork.Core.Models;

namespace StaffWork.Infrastructure.Implementations
{
    public class AdministrationService : ServicesBase<Administration>
    {
        public AdministrationService(IGenericRepo<Administration> genericRepo) : base(genericRepo)
        {
        }

        public override Task InsertAsync(Administration model)
        {
            model.IsActive = true;
            model.DateCreated = DateTime.Now;
            return base.InsertAsync(model);
        }

        public override Task UpdateAsync(int Id, Administration model)
        {
            model.DateModified = DateTime.Now;
            return base.UpdateAsync(Id, model);
        }
    }
}
