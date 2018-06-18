using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpecLab.Business;
using SpecLab.Business.BusinessEnum;

namespace SpecLab.FrontEnd.Models
{
    public class SampleTypeSelectItem
    {
        public SampleType Code { get; set; }

        public string Description { get { return EnumUtils.GetEnumDescription(Code); } }

        public SampleTypeSelectItem(SampleType sampleType)
        {
            this.Code = sampleType;
        }
    }
}