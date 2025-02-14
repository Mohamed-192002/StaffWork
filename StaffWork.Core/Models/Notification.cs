using StaffWork.Core.Models.Base;

namespace StaffWork.Core.Models
{
    public class Notification : BaseEntity
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public bool IsRead { get; set; }
        public int VacationId { get; set; }
        public Vacation Vacation { get; set; }
    }
}
