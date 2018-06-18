using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecLab.Business.BusinessObjects
{
    public class ExportNote
    {
        public string ExportId { get; set; }
        public string ExportUserId { get; set; }
        public string ExportForUserId { get; set; }
        public string ExportReason { get; set; }
        public DateTime ExportDate { get; set; }
        public string ExportDateDisplay
        { 
            get { return ExportDate.ToString(CommonConstant.DateTimeFormatDisplay); }
        }
        public int NumberExport { get { return this.ExportNoteDetails.Count; } }

        public List<ExportNoteDetail> ExportNoteDetails { get; set; }

        public ExportNote()
        {
            this.ExportNoteDetails = new List<ExportNoteDetail>();
        }
    }
}
