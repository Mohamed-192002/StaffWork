using System.ComponentModel.DataAnnotations.Schema;
using StaffWork.Core.Models.Base;

namespace StaffWork.Core.Models
{
    public class Administration : BaseEntity
    {
        public virtual string Name { get; set; }
        public virtual ICollection<Department> Departments { get; set; } = [];
        public virtual string? ManagerId { get; set; }
        [ForeignKey(nameof(ManagerId))]
        public virtual User? Manager { get; set; }
    }
}