using Microsoft.AspNetCore.Mvc.Rendering;
using StaffWork.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffWork.Core.Paramaters
{
    public class WorkDailyEditFormViewModel
    {
        public virtual int Id { get; set; }
        [Display(Name = "ملاحظات")]
        public string? Note { get; set; }

        [Required(ErrorMessage = "الحقل مطلوب")]
        [Display(Name = "العمل")]
        public int WorkTypeId { get; set; }
        public IEnumerable<SelectListItem>? WorkTypes { get; set; }
    }
}
