using System.ComponentModel.DataAnnotations;

namespace StaffWork.Core.Paramaters
{
    public class VacationTypeViewModel
    {
        public virtual int Id { get; set; }

        [MaxLength(500, ErrorMessage = "لا يمكن أن يكون الحد الأقصى للطول أكثر من 500 حروف.")]
        [Display(Name = "نوع الاجازه")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        public virtual string Name { get; set; }
    }
}
