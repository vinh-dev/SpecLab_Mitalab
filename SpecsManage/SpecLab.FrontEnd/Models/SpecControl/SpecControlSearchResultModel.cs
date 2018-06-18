using SpecLab.Business;
using SpecLab.Business.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpecLab.FrontEnd.Models.SpecControl
{
    public class SpecControlSearchResultModel : CommonModelResult
    {
        public List<TubeSampleSearchDataItem> ListData { get; set; }

        public SpecControlSearchResultModel()
        {
            this.ListData = new List<TubeSampleSearchDataItem>();
        }
    }
}