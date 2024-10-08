using StaffWork.Core.Interfaces;
using StaffWork.Core.Models;

namespace StaffWork.Infrastructure.Implementations
{
    public class UserService : ServicesBase<User>
    {
        public UserService(IGenericRepo<User> genericRepo) : base(genericRepo)
        {
        }
        public override Task InsertAsync(User model)
        {
            model.IsActive = true;
            model.DateCreated = DateTime.Now;
            return base.InsertAsync(model);
        }

        public override Task UpdateAsync(int Id, User model)
        {
            model.DateModified = DateTime.Now;
            return base.UpdateAsync(Id, model);
        }
    }
}
