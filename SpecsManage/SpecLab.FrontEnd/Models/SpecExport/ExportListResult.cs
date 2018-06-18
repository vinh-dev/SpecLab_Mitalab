using SpecLab.Business;
using SpecLab.Business.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpecLab.FrontEnd.Models.SpecExport
{
    public class ExportListResult : CommonModelResult
    {
        public List<ExportNoteShortData> ListData { get; set; }

        public ExportListResult()
        {
            this.ListData = new List<ExportNoteShortData>();
        }
    }
}