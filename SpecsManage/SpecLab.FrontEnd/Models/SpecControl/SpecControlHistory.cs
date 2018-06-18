using SpecLab.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpecLab.Business.BusinessObjects;

namespace SpecLab.FrontEnd.Models.SpecControl
{
    public class SpecControlHistory : CommonModelResult
    {
        public TubeSampleSpecInfo SampleSpecInfo { get; set; }

        public List<SampleHistoryInfo> HistoryInfos { get; set; }
    }
}