using StaffWork.Core.Models.Base;

namespace StaffWork.Core.Models
{
    public class Employee : BaseEntity
    {
        public string FullName { get; set; }
        public string? Court { get; set; }
        public string? Appeal { get; set; }

    }
}
