using SpecLab.Business;
using SpecLab.Business.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpecLab.Business.BusinessObjects.Export;

namespace SpecLab.FrontEnd.Models.SpecExport
{
    public class ExportCountResponse : CommonModelResult
    {
        public List<TubeExportCount> CountDetails { get; set; }

        public ExportCountResponse()
        {
            CountDetails = new List<TubeExportCount>();
        }
    }
}