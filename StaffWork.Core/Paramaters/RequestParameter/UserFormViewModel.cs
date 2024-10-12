using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace StaffWork.Core.Paramaters
{
    public class UserFormViewModel
    {
        public string? Id { get; set; }
        [MaxLength(20, ErrorMessage = "لا يمكن أن يكون الحد الأقصى للطول أكثر من 20 حروف."), Display(Name = "اسم المستخدم")]
        [Remote("AllowUserName", null!, AdditionalFields = "Id", ErrorMessage = "اسم المستخدم موجود")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        public string UserName { get; set; }
        [MaxLength(100, ErrorMessage = "لا يمكن أن يكون الحد الأقصى للطول أكثر من 100 حروف."), Display(Name = "الاسم بالكامل")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        public string FullName { get; set; }
        [DataType(DataType.Password), Display(Name = "كلمه المرور")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        [MinLength(8, ErrorMessage = "يجب أن تتكون كلمات المرور من 8 أحرف على الأقل")]
        public string Password { get; set; }

        [Required(ErrorMessage = "الحقل مطلوب")]
        [Display(Name = "القسم")]
        public int DepartmentId { get; set; }
        public IEnumerable<SelectListItem>? Departments { get; set; }


        [Display(Name = "الرتبه")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        public string SelectedRole { get; set; }

        public IEnumerable<SelectListItem>? Roles { get; set; }
    }
    public class UpdateUserFormViewModel
    {
        public string? Id { get; set; }
        [MaxLength(20, ErrorMessage = "لا يمكن أن يكون الحد الأقصى للطول أكثر من 20 حروف."), Display(Name = "اسم المستخدم")]
        [Remote("AllowUserName", null!, AdditionalFields = "Id", ErrorMessage = "اسم المستخدم موجود")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        public string UserName { get; set; }
        [MaxLength(100, ErrorMessage = "لا يمكن أن يكون الحد الأقصى للطول أكثر من 100 حروف."), Display(Name = "الاسم بالكامل")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        public string FullName { get; set; }
        [DataType(DataType.Password), Display(Name = "كلمه المرور")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        [MinLength(8, ErrorMessage = "يجب أن تتكون كلمات المرور من 8 أحرف على الأقل")]
        public string Password { get; set; }

        [Display(Name = "القسم")]
        public int? DepartmentId { get; set; }
        public IEnumerable<SelectListItem>? Departments { get; set; }

        [Display(Name = "الرتبه")]
        public string? SelectedRole { get; set; }

        public IEnumerable<SelectListItem>? Roles { get; set; }
    }
}
