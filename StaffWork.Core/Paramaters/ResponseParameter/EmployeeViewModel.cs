using System.ComponentModel.DataAnnotations;

namespace StaffWork.Core.Paramaters
{
    public class EmployeeViewModel
    {
        public virtual int Id { get; set; }

        [MaxLength(500, ErrorMessage = "لا يمكن أن يكون الحد الأقصى للطول أكثر من 500 حروف.")]
        [Display(Name = "اسم الموظف")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        public virtual string FullName { get; set; }
        [MaxLength(500, ErrorMessage = "لا يمكن أن يكون الحد الأقصى للطول أكثر من 500 حروف.")]
        [Display(Name = "المحكمه")]
        public virtual string? Court { get; set; }
        [MaxLength(500, ErrorMessage = "لا يمكن أن يكون الحد الأقصى للطول أكثر من 500 حروف.")]
        [Display(Name = "الاستئتاف")]
        public virtual string? Appeal { get; set; }
    }
}
