using SpecLab.Business;
using SpecLab.Business.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpecLab.FrontEnd.Models.SpecImport
{
    public class ImportInfo : CommonModelResult
    {
        public ImportNote ImportNote { get; set; }
    }
}