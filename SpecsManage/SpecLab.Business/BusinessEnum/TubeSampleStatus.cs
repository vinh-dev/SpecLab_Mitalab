using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecLab.Business.BusinessEnum
{
    public enum TubeSampleStatus
    {
        [Description("Tốt")]
        Good = 0,

        [Description("Hủy")]
        Remove = 1,

        [Description("Hỏng")]
        Corrupt = 2,

        [Description("Quá lần xuất")]
        MoreTime = 3
    }
}
