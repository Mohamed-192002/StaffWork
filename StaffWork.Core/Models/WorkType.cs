using System.ComponentModel.DataAnnotations.Schema;
using StaffWork.Core.Models.Base;

namespace StaffWork.Core.Models
{
    public class WorkType : BaseEntity
    {
        public virtual string Name { get; set; }
        public int? AdministrationId { get; set; }
        [ForeignKey(nameof(AdministrationId))]
        public Administration? Administration { get; set; }
        public int? DepartmentId { get; set; }
        [ForeignKey(nameof(DepartmentId))]
        public Department? Department { get; set; }
    }
}
