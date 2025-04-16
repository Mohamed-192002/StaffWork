using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StaffWork.Core.Models
{
    public class User : IdentityUser
    {
        [MaxLength(100)]
        public string FullName { get; set; }
        [MaxLength(500)]
        public string? ImageUrl { get; set; }
        public string Password { get; set; }
        public virtual DateTime DateCreated { get; set; }
        public virtual DateTime? DateModified { get; set; }
        public virtual bool IsActive { get; set; }
        public int? AdministrationId { get; set; }
        public Administration? Administration { get; set; }
        public virtual int? DepartmentId { get; set; }
        [ForeignKey(nameof(DepartmentId))]
        public virtual Department? Department { get; set; }

        public virtual ICollection<WorkDaily> WorkDailies { get; set; } = [];

        public int? DepartmentAdminId { get; set; }
        [ForeignKey(nameof(DepartmentAdminId))]
        public virtual DepartmentAdmin? DepartmentAdmin { get; set; }
    }
}
