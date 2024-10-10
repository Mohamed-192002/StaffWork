using StaffWork.Core.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace StaffWork.Core.Models
{
	public class Department : BaseEntity
	{
		public virtual string Name { get; set; }

		public virtual ICollection<User> Users { get; set; } = [];

        //public virtual string AdminId { get; set; }
        //[ForeignKey(nameof(AdminId))]
        //public virtual User? Admin { get; set; }
    }
}
