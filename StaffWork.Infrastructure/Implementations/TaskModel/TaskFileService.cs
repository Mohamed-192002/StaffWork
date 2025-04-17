using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StaffWork.Core.Interfaces;
using StaffWork.Core.Models;

namespace StaffWork.Infrastructure.Implementations
{
    public class TaskFileService : ServicesBase<TaskFile>
    {
        public TaskFileService(IGenericRepo<TaskFile> genericRepo) : base(genericRepo)
        {
        }
        public override Task InsertAsync(TaskFile model)
        {
            model.IsActive = true;
            model.DateCreated = DateTime.Now;
            return base.InsertAsync(model);
        }

        public override Task UpdateAsync(int Id, TaskFile model)
        {
            model.DateModified = DateTime.Now;
            return base.UpdateAsync(Id, model);
        }
    }
}
