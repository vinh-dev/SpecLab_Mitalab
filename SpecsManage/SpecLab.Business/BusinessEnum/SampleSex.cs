using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecLab.Business.BusinessEnum
{
    public enum SampleSex
    {
        //[Description("Không xác định")]
        [Description("?")]
        NotSpecifiy = '?', 

        [Description("Nữ")]
        Female = 'F',

        [Description("Nam")]
        Male = 'M'
    }
}
