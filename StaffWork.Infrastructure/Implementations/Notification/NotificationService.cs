using StaffWork.Core.Interfaces;
using StaffWork.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffWork.Infrastructure.Implementations
{
    public class NotificationService : ServicesBase<Notification>
    {
        public NotificationService(IGenericRepo<Notification> genericRepo) : base(genericRepo)
        {
        }

        public override Task InsertAsync(Notification model)
        {
            model.IsActive = true;
            model.DateCreated = DateTime.Now;
            return base.InsertAsync(model);
        }

        public override Task UpdateAsync(int Id, Notification model)
        {
            model.DateModified = DateTime.Now;
            return base.UpdateAsync(Id, model);
        }
    }
}
