using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpecLab.Business.BusinessEnum;
using SpecLab.Business.Database;

namespace SpecLab.Business.BusinessObjects
{
    public class ShortStorageInfo
    {
        public string StorageId { get; set; }

        public int NumberStorage { get; set; }
        public int? NumRows { get; set; }
        public int? NumColumn { get; set; }

        public override string ToString()
        {
            return string.Format("{0} ({1}) ({2}x{3})", StorageId, NumberStorage, NumRows, NumColumn);
        }
    }
}
