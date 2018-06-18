using SpecLab.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpecLab.FrontEnd.Models.SpecControl
{
    public class SpecControlModel
    {
        public DateTime? EndSearchDate { get; set; }

        public DateTime? StartSearchDate { get; set; }

        public List<TubeStatusSelectItem> TubeStatusList { get; set; }

        public string EndSearchDateString
        { 
            get
            {
                if (EndSearchDate != null)
                {
                    return EndSearchDate.GetValueOrDefault().ToString(CommonConstant.DateFormatDisplay);
                }
                return string.Empty;
            }
        }

        public string StartSearchDateString
        { 
            get
            {
                if (StartSearchDate != null)
                {
                    return StartSearchDate.GetValueOrDefault().ToString(CommonConstant.DateFormatDisplay);
                }
                return string.Empty;
            }
        }

        public SpecControlModel()
        {
            this.TubeStatusList = new List<TubeStatusSelectItem>();
        }
    }
}