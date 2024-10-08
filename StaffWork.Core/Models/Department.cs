using StaffWork.Core.Models.Base;

namespace StaffWork.Core.Models
{
	public class Department : BaseEntity
	{
		public virtual string Name { get; set; }

	//	public virtual ICollection<User> Employees { get; set; } = [];
	}
}
