using StaffWork.Core.Interfaces;
using StaffWork.Core.Models;

namespace StaffWork.Infrastructure.Implementations
{
    public class TaskModelService : ServicesBase<TaskModel>
    {
        public TaskModelService(IGenericRepo<TaskModel> genericRepo) : base(genericRepo)
        {
        }
        public override Task InsertAsync(TaskModel model)
        {
            model.IsActive = true;
            model.DateCreated = DateTime.Now;
            return base.InsertAsync(model);
        }

        public override Task UpdateAsync(int Id, TaskModel model)
        {
            model.DateModified = DateTime.Now;
            return base.UpdateAsync(Id, model);
        }
    }
}
