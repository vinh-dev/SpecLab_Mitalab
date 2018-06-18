using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecLab.Business.BusinessObjects
{
    public class RemovalNote
    {
        public string RemovalId { get; set; }
        public string RemovalUserId { get; set; }
        public string RemovalReason { get; set; }
        public DateTime RemovalDate { get; set; }
        public string RemovalDateDisplay
        {
            get { return RemovalDate.ToString(CommonConstant.DateTimeFormatDisplay); }
        }
        public int NumberRemoval { get { return this.RemovalNoteDetails.Count; } }
        public List<RemovalNoteDetail> RemovalNoteDetails { get; set; }

        public RemovalNote()
        {
            this.RemovalNoteDetails = new List<RemovalNoteDetail>();
        }
    }
}
