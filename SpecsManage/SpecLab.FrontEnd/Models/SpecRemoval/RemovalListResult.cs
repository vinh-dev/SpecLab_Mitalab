using SpecLab.Business;
using SpecLab.Business.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpecLab.FrontEnd.Models.SpecRemoval
{
    public class RemovalListResult : CommonModelResult
    {
        public List<RemovalNoteShortData> ListData { get; set; }

        public RemovalListResult()
        {
            this.ListData = new List<RemovalNoteShortData>();
        }
    }
}