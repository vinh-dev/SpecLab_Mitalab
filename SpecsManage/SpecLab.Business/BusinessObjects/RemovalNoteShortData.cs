using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecLab.Business.BusinessObjects
{
    public class RemovalNoteShortData
    {
        public string RemovalId { get; set; }
        public string RemovalUserId { get; set; }
        public string RemovalReason { get; set; }
        public DateTime RemovalDate { get; set; }
        public string RemovalDateDisplay
        {
            get { return this.RemovalDate.ToString(CommonConstant.DateTimeFormatDisplay); }
        }
        public int NumberRemoval { get; set; }
    }
}
