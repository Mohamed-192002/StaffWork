using StaffWork.Core.Consts;
using StaffWork.Core.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace StaffWork.Core.Models
{
    public class WorkDaily : BaseEntity
    {
        public string? Note { get; set; }
        public virtual DateTime Date { get; set; }

        public virtual int WorkTypeId { get; set; }
        [ForeignKey(nameof(WorkTypeId))]
        public virtual WorkType? WorkType { get; set; }

        public virtual string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User? User { get; set; }
        public virtual Status Status { get; set; } 
    }
}