using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecLab.Business.BusinessEnum
{
    public enum CalculatedCondition
    {
        //[Description("Tốt")]
        //Good = 0,

        [Description("Đang xuất, tốt")]
        InUse = 1,

        [Description("Hủy")]
        Remove = 2,

        [Description("Hỏng")]
        Corrupt = 3,

        [Description("Hết")]
        OutOfVolume = 4,

        [Description("Trong kho, tốt")]
        InStorage = 5,

        [Description("Quá lần xuất")]
        MoreTime = 6,
    }
}
