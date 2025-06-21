using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
namespace StaffWork.Core.Paramaters
{
    public class PersonalReminderFormViewModel
    {
        public int Id { get; set; }
        [Display(Name = "تاريخ التذكير")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        public DateTime ReminderDate { get; set; }
        [Display(Name = "اسم التذكير")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        public string Title { get; set; }
        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; } = string.Empty;

        [Display(Name = "ملفات")]
        public IList<IFormFile>? PersonalReminderFormFiles { get; set; } = [];
        public IList<TaskFileDisplay> ExistingFiles { get; set; } = [];
        public string? DeletedFileUrls { get; set; }
    }
}
