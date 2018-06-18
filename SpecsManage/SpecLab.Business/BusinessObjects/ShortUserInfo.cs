using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpecLab.Business.BusinessEnum;
using SpecLab.Business.Database;

namespace SpecLab.Business.BusinessObjects
{
    public class ShortUserInfo
    {
        public string UserId { get; set; }

        public string FullName { get; set; }

        public string DisplayName
        {
            get { return string.Format("{0}: {1}", UserId, FullName); }
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
