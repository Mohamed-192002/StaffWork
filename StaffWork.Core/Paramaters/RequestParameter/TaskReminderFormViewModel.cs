using System.ComponentModel.DataAnnotations;

namespace StaffWork.Core.Paramaters
{
    public class TaskReminderFormViewModel
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
        public int TaskModelId { get; set; }
    }
}
