using StaffWork.Core.Consts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffWork.Infrastructure.Helpers
{
    public static class StatusLocalization
    {
        public static readonly Dictionary<Status, string> StatusInArabic = new Dictionary<Status, string>
    {
        { Status.Pending, "قيد الانتظار" },
        { Status.Accepted, "مقبول" },
        { Status.Rejected, "مرفوض" }
    };

        public static string GetStatusInArabic(Status status)
        {
            return StatusInArabic.ContainsKey(status) ? StatusInArabic[status] : status.ToString();
        }
    }
}
