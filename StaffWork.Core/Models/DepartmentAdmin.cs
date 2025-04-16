using StaffWork.Core.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace StaffWork.Core.Models
{
    public class DepartmentAdmin : BaseEntity
    {
        public int DepartmentId { get; set; }
        public virtual Department? Department { get; set; }
        public virtual string? AdminId { get; set; }
        [ForeignKey(nameof(AdminId))]
        public virtual User? Admin { get; set; }
    }
}