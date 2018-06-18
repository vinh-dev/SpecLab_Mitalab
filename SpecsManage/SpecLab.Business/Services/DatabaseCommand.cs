using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecLab.Business.Services
{
    public class DatabaseCommand<InVal, ReturnVal>
    {
        public InVal CallingInfo { get; set; }
        public ReturnVal ReturnValue { get; set; }
    }
}
