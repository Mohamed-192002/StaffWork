using StaffWork.Core.Interfaces;
using StaffWork.Core.Models;

namespace StaffWork.Infrastructure.Implementations
{
    public class TaskReminderService : ServicesBase<TaskReminder>
    {
        public TaskReminderService(IGenericRepo<TaskReminder> genericRepo) : base(genericRepo)
        {
        }
        public override Task InsertAsync(TaskReminder model)
        {
            model.IsActive = true;
            model.DateCreated = DateTime.Now;
            return base.InsertAsync(model);
        }

        public override Task UpdateAsync(int Id, TaskReminder model)
        {
            model.DateModified = DateTime.Now;
            return base.UpdateAsync(Id, model);
        }
    }
}
