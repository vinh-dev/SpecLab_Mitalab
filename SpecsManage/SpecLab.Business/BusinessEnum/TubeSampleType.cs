using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecLab.Business.BusinessEnum
{
    public enum TubeSampleType
    {
        [Description("Trong kho")]
        InStorage = 0,
       
        [Description("Đang xuất")]
        InUse = 1
    }
}
