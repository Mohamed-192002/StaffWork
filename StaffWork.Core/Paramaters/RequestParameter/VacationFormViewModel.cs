using Microsoft.AspNetCore.Mvc.Rendering;
using StaffWork.Core.Consts;
using StaffWork.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace StaffWork.Core.Paramaters
{
    public class VacationFormViewModel
    {
        public virtual int Id { get; set; }

        [Required(ErrorMessage = "الحقل مطلوب")]
        [Display(Name = "الموظف")]
        public int EmployeeId { get; set; }
        public IEnumerable<SelectListItem>? Employees { get; set; }
        
        [Required(ErrorMessage = "الحقل مطلوب")]
        [Display(Name = "نوع الاجازه")]
        public int VacationTypeId { get; set; }
        public IEnumerable<SelectListItem>? VacationTypes { get; set; }
        [Required(ErrorMessage = "الحقل مطلوب")]
        [Display(Name = "بدايه الاجازه")]
        public virtual DateTime StartDate { get; set; } = DateTime.Now;
        [Display(Name = "مده الاجازه")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        public int? VacationDays { get; set; }
        [Display(Name = "سبب الاجازه")]
        public string? Description { get; set; }
        [Display(Name = "هل تم العوده؟")]
        public bool IsReturned { get; set; }

        [Display(Name = "تاريخ العوده")]
        public virtual DateTime? ReturnedDate { get; set; }

        [Display(Name = "فتره الاجازه")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        public VacationDuration VacationDuration { get; set; }
    }
}
