using StaffWork.Core.Interfaces;
using StaffWork.Core.Models;
namespace StaffWork.Infrastructure.Implementations
{
    public class PersonalReminderFileService : ServicesBase<PersonalReminderFile>
    {
        public PersonalReminderFileService(IGenericRepo<PersonalReminderFile> genericRepo) : base(genericRepo)
        {
        }
        public override Task InsertAsync(PersonalReminderFile model)
        {
            model.IsActive = true;
            model.DateCreated = DateTime.Now;
            return base.InsertAsync(model);
        }

        public override Task UpdateAsync(int Id, PersonalReminderFile model)
        {
            model.DateModified = DateTime.Now;
            return base.UpdateAsync(Id, model);
        }
    }
}
