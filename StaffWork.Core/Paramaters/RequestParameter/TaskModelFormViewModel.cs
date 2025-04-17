using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace StaffWork.Core.Paramaters
{
    public class TaskModelFormViewModel
    {
        public virtual int Id { get; set; }
        [Display(Name = "اسم المهمه")]
        public string? Title { get; set; }
        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        [Display(Name = "الموظفين")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        public IList<string> SelectedUsers { get; set; } = [];
        public IEnumerable<SelectListItem>? Users { get; set; }

        [Display(Name = "ملفات المهمه")]
        public IList<IFormFile>? TaskFiles { get; set; } = [];
        public IList<TaskFileDisplay> ExistingFiles { get; set; } = [];
        public string? DeletedFileUrls { get; set; }
    }

    public class TaskFileDisplay
    {
        public string FileUrl { get; set; } = null!;
        public string FileName { get; set; } = null!;
    }
}
