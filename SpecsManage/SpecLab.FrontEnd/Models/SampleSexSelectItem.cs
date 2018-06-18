using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpecLab.Business;
using SpecLab.Business.BusinessEnum;

namespace SpecLab.FrontEnd.Models
{
    public class SampleSexSelectItem
    {
        public SampleSex Code { get; set; }

        public string Description { get { return EnumUtils.GetEnumDescription(Code); } }

        public SampleSexSelectItem(SampleSex sex)
        {
            this.Code = sex;
        }
    }
}