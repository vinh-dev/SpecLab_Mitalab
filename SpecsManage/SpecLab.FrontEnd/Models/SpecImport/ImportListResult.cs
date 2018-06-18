using SpecLab.Business;
using SpecLab.Business.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpecLab.FrontEnd.Models.SpecImport
{
    public class ImportListResult : CommonModelResult
    {
        public List<ImportNoteShortData> ListData { get; set; }

        public ImportListResult()
        {
            this.ListData = new List<ImportNoteShortData>();
        }
    }
}