using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecLab.Business.BusinessObjects
{
    public class ImportNoteShortData
    {
        public string ImportId { get; set; }
        public string ImportUserId { get; set; }
        public DateTime ImportDate { get; set; }
        public string ImportDateDisplay
        {
            get { return this.ImportDate.ToString(CommonConstant.DateTimeFormatDisplay); }
        }
        public int NumberImport { get; set; }
    }
}
