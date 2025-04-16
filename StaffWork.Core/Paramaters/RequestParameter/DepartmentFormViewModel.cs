using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace StaffWork.Core.Paramaters
{
    public class DepartmentFormViewModel
    {
        public virtual int Id { get; set; }

        [MaxLength(500, ErrorMessage = "لا يمكن أن يكون الحد الأقصى للطول أكثر من 100 حروف.")]
        [Display(Name = "اسم القسم")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        public virtual string Name { get; set; }

        [Required(ErrorMessage = "الحقل مطلوب")]
        [Display(Name = "الاداره")]
        public int AdministrationId { get; set; }
        public IEnumerable<SelectListItem>? Administrations { get; set; }
    }
}
