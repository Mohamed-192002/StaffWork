
using System.ComponentModel.DataAnnotations;


namespace StaffWork.Core.Models.Base
{
    public class BaseEntity
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual DateTime DateCreated { get; set; }
        public virtual DateTime? DateModified { get; set; }
        public virtual bool IsActive { get; set; }
    }
}
