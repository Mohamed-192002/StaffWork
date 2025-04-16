using System.ComponentModel.DataAnnotations;


namespace StaffWork.Core.Paramaters
{
    public class DepartmentViewModel
    {
        public virtual int Id { get; set; }

        [Display(Name = "اسم القسم")]
        public virtual string Name { get; set; }

        [Display(Name = "الاداره")]
        public string AdministrationName { get; set; }

        [Display(Name = "مدير القسم")]
        public string AdminName { get; set; }
    }
}
