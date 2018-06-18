using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecLab.Business.BusinessEnum
{
    public enum HistoryAction
    {
        [Description("Nhập")]
        Import = 0,

        [Description("Xuất")]
        Export = 1,

        [Description("Tái nhập")]
        Update = 2,

        [Description("Hủy")]
        Remove = 3
    }
}
