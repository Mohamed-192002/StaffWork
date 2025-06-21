using StaffWork.Core.Interfaces;
using StaffWork.Core.Models;

namespace StaffWork.Infrastructure.Implementations
{
    public class PersonalReminderService : ServicesBase<PersonalReminder>
    {
        public PersonalReminderService(IGenericRepo<PersonalReminder> genericRepo) : base(genericRepo)
        {
        }
        public override Task InsertAsync(PersonalReminder model)
        {
            model.IsActive = true;
            model.DateCreated = DateTime.Now;
            return base.InsertAsync(model);
        }

        public override Task UpdateAsync(int Id, PersonalReminder model)
        {
            model.DateModified = DateTime.Now;
            return base.UpdateAsync(Id, model);
        }
    }
}
