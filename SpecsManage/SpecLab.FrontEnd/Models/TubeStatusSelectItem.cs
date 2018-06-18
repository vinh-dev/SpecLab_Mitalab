using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpecLab.Business;
using SpecLab.Business.BusinessEnum;

namespace SpecLab.FrontEnd.Models
{
    public class TubeStatusSelectItem
    {
        public TubeSampleStatus Status { get; set; }

        public string Description { get { return EnumUtils.GetEnumDescription(Status); } }

        public TubeStatusSelectItem(TubeSampleStatus status)
        {
            this.Status = status;
        }
    }
}