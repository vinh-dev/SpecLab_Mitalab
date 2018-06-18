using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpecLab.Business.BusinessEnum;
using SpecLab.Business.Database;

namespace SpecLab.Business.BusinessObjects
{
    public class ContentInfo
    {
        public string ContentId { get; set; }

        public string ContentText { get; set; }

        public override string ToString()
        {
            return string.Format("{0}: {1}", ContentId, ContentText);
        }
    }
}
