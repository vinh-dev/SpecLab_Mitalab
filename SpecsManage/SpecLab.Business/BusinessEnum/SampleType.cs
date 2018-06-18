using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecLab.Business.BusinessEnum
{
    public enum SampleType
    {
        [Description("Huyết tương")]
        Plasma = 0,

        [Description("Huyết thanh")]
        Serum = 1,

        [Description("PBMC")]
        PBMC = 2,
    }
}
