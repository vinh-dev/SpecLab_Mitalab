using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecLab.Business.BusinessObjects
{
    public class ExportNoteShortData
    {
        public string ExportId { get; set; }
        public string ExportUserId { get; set; }
        public string ExportForUserId { get; set; }
        public string ExportReason { get; set; }
        public DateTime ExportDate { get; set; }
        public string ExportDateDisplay
        {
            get { return this.ExportDate.ToString(CommonConstant.DateTimeFormatDisplay); }
        }
        public int NumberExport { get; set; }
    }
}
