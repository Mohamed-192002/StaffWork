using Microsoft.AspNetCore.Mvc.Rendering;
using StaffWork.Core.Consts;
using System.ComponentModel.DataAnnotations;

namespace StaffWork.Core.Paramaters
{
    public class VacationViewModel
    {
        public virtual int Id { get; set; }

        [Display(Name = "اسم الموظف")]
        public string EmployeeName { get; set; }
        [Display(Name = "دار القضاء")]
        public string? Court { get; set; }
        [Display(Name = "الاستئناف")]
        public string? Appeal { get; set; }

        [Display(Name = "نوع الاجازه")]
        public string VacationType { get; set; }

        [Display(Name = "بدايه الاجازه")]
        public virtual DateTime StartDate { get; set; }
        public virtual DateTime EndDate { get; set; }
        [Display(Name = "مده الاجازه")]
        public int VacationDays { get; set; }
        [Display(Name = "فتره الاجازه")]
        public VacationDuration VacationDuration { get; set; }

        [Display(Name = "سبب الاجازه")]
        public string? Description { get; set; }
        [Display(Name = "هل تم العوده؟")]
        public bool IsReturned { get; set; }

        [Display(Name = "تاريخ العوده")]
        public virtual DateTime? ReturnedDate { get; set; }

        ////////////////////
        public bool IsAutoNotifi { get; set; }
        public VacationDuration? CustomNotifiDuration { get; set; }
        public int? CustomNotifiBeforeDays { get; set; }
        public virtual DateTime CustomNotifiDate { get; set; } = DateTime.Now;
    }
}
