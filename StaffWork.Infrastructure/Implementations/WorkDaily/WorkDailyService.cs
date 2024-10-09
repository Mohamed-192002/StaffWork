using StaffWork.Core.Interfaces;
using StaffWork.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffWork.Infrastructure.Implementations
{
    internal class WorkDailyService : ServicesBase<WorkDaily>
    {
        public WorkDailyService(IGenericRepo<WorkDaily> genericRepo) : base(genericRepo)
        {
        }

        public override Task InsertAsync(WorkDaily model)
        {
            model.IsActive = true;
            model.DateCreated = DateTime.Now;
            return base.InsertAsync(model);
        }

        public override Task UpdateAsync(int Id, WorkDaily model)
        {
            model.DateModified = DateTime.Now;
            return base.UpdateAsync(Id, model);
        }
    }
}
