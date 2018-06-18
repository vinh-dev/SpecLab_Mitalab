using SpecLab.Business;
using SpecLab.Business.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpecLab.FrontEnd.Models.SpecExport
{
    public class ExportInfo : CommonModelResult
    {
        public ExportNote ExportNote { get; set; }
    }
}