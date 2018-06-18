using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecLab.Business.BusinessObjects
{
    public class ImportNote
    {
        public string ImportId { get; set; }
        public string ImportUserId { get; set; }
        public DateTime ImportDate { get; set; }
        public string ImportDateDisplay
        { 
            get { return ImportDate.ToString(CommonConstant.DateTimeFormatDisplay); }
        }
        public int NumberImport { get { return this.ImportNoteDetails.Count; } }

        public List<ImportNoteDetail> ImportNoteDetails { get; set; }

        public ImportNote()
        {
            this.ImportNoteDetails = new List<ImportNoteDetail>();
        }
    }
}
