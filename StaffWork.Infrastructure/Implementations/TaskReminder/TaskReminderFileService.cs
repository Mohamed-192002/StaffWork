using StaffWork.Core.Interfaces;
using StaffWork.Core.Models;

namespace StaffWork.Infrastructure.Implementations
{
    public class TaskReminderFileService : ServicesBase<TaskReminderFile>
    {
        public TaskReminderFileService(IGenericRepo<TaskReminderFile> genericRepo) : base(genericRepo)
        {
        }
        public override Task InsertAsync(TaskReminderFile model)
        {
            model.IsActive = true;
            model.DateCreated = DateTime.Now;
            return base.InsertAsync(model);
        }

        public override Task UpdateAsync(int Id, TaskReminderFile model)
        {
            model.DateModified = DateTime.Now;
            return base.UpdateAsync(Id, model);
        }
    }
}
