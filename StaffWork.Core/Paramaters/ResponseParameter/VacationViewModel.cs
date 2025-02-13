using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace StaffWork.Core.Paramaters
{
    public class VacationViewModel
    {
        public virtual int Id { get; set; }

        [Display(Name = "اسم الموظف")]
        public string EmployeeName { get; set; }

        [Display(Name = "نوع الاجازه")]
        public string VacationType { get; set; }

        [Display(Name = "بدايه الاجازه")]
        public virtual DateTime StartDate { get; set; }
        [Display(Name = "ايام الاجازه")]
        public int VacationDays { get; set; }
        [Display(Name = "سبب الاجازه")]
        public string? Description { get; set; }
        [Display(Name = "هل تم العوده؟")]
        public bool IsReturned { get; set; }

        [Display(Name = "تاريخ العوده")]
        public virtual DateTime? ReturnedDate { get; set; }
    }
}
