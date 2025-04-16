using System.ComponentModel.DataAnnotations.Schema;
using StaffWork.Core.Models.Base;

namespace StaffWork.Core.Models
{
    public class Employee : BaseEntity
    {
        public string FullName { get; set; }
        public int? AdministrationId { get; set; }
        [ForeignKey(nameof(AdministrationId))]
        public Administration? Administration { get; set; }
        public int? DepartmentId { get; set; }
        [ForeignKey(nameof(DepartmentId))]
        public Department? Department { get; set; }


        public string? Court { get; set; }
        public string? Appeal { get; set; }
    }
}
