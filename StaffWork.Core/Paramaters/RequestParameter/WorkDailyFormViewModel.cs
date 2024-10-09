using Microsoft.AspNetCore.Mvc.Rendering;
using StaffWork.Core.Models;

namespace StaffWork.Core.Paramaters
{
    public class WorkDailyFormViewModel
    {
        public User User { get; set; }
        public IList<WorkDaily> WorkDailies { get; set; }
        public IEnumerable<SelectListItem> WorkTypes { get; set; }
        public DateViewModel Date { get; set; }

        public WorkDailyFormViewModel()
        {
            // Ensure the collections are instantiated to avoid null reference issues
            WorkDailies = new List<WorkDaily>();
            WorkTypes = new List<SelectListItem>();
        }
    }
}
