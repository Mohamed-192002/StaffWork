using System.ComponentModel.DataAnnotations;

namespace StaffWork.Core.Paramaters
{
    public class AdministrationViewModel
    {
        public virtual int Id { get; set; }

        [MaxLength(500, ErrorMessage = "لا يمكن أن يكون الحد الأقصى للطول أكثر من 100 حروف.")]
        [Display(Name = "اسم الاداره")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        public virtual string Name { get; set; }
        [Display(Name = "مدير الاداره")]
        public virtual string AdminName { get; set; }
    }
}
