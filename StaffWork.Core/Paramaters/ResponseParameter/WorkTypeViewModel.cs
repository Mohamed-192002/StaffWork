using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffWork.Core.Paramaters
{
    public class WorkTypeViewModel
    {
        public virtual int Id { get; set; }

        [MaxLength(500, ErrorMessage = "لا يمكن أن يكون الحد الأقصى للطول أكثر من 100 حروف.")]
        [Display(Name = "اسم العمل")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        public virtual string Name { get; set; }
    }
}
