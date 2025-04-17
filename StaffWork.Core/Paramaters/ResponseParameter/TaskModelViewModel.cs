
using StaffWork.Core.Models;

namespace StaffWork.Core.Paramaters
{
    public class TaskModelViewModel
    {
        public virtual int Id { get; set; }
        public string? Title { get; set; }
        public string? Notes { get; set; }
        //public string? FilePath { get; set; }
        public bool IsReceived { get; set; }
        public DateTime? DateReceived { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? DateCompleted { get; set; }
        public List<string> Users { get; set; }
    }
}
