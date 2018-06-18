using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpecLab.FrontEnd.Models.SpecExport
{
    public class ExportSampleRequest
    {
        public string ExportId { get; set; }
        public string ExportUserId { get; set; }
        public string ExportForUserId { get; set; }
        public string ExportReason { get; set; }
        public List<string> TubeExportIds { get; set; }

        public ExportSampleRequest()
        {
            TubeExportIds = new List<string>();
        }
    }
}