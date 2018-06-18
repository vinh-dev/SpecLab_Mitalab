using SpecLab.Business;
using SpecLab.Business.BusinessEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpecLab.FrontEnd.Models.SpecControl
{
    public class SpecControlSearchCriteria
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string SpecId { get; set; }
        public string TubeId { get; set; }
        public string StorageId { get; set; }
        public int? LocationId { get; set; }

        public List<TubeSampleStatus> FilterStatus { get; set; }
        public List<TubeSampleType> FilterType { get; set; }

        public SpecControlSearchCriteria()
        {
            FilterStatus = new List<TubeSampleStatus>();
            FilterType = new List<TubeSampleType>();
        }
    }
}