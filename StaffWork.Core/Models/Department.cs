using StaffWork.Core.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace StaffWork.Core.Models
{
    public class Department : BaseEntity
    {
        public virtual string Name { get; set; }
        public int? AdministrationId { get; set; }
        [ForeignKey(nameof(AdministrationId))]
        public virtual Administration? Administration { get; set; }
        public int? DepartmentAdminId { get; set; }
        [ForeignKey(nameof(DepartmentAdminId))]
        public virtual DepartmentAdmin? DepartmentAdmin { get; set; }

        public virtual ICollection<User> Users { get; set; } = [];
    }
}