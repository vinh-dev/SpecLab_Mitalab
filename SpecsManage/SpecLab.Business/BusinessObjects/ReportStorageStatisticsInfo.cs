using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpecLab.Business.BusinessEnum;

namespace SpecLab.Business.BusinessObjects
{
    public class ReportStorageStatisticsInfo
    {
        public string StorageId { get; set; }

        public int MaximumStorage { get; set; }

        public int LocationNum { get; set; }

        public string StorageDisplay
        {
            get
            {
                return string.Format("{0} ({1})", StorageId, MaximumStorage);
            }
        }

        public string LocationNumDisplay
        {
            get { return string.Format("{0}", LocationNum); }
        }

        public string TubeId { get; set; }

        public TubeSampleStatus Status { get; set; }

        public TubeSampleType Type { get; set; }

        public string StatusDisplay { get { return EnumUtils.GetEnumDescription(Status); } }

        public CalculatedCondition Condition
        {
            get { return CommonUtils.GetCalculatedCondition(Status,Type, Volume, NumberExport); }
        }

        public double Volume { get; set; }

        public int NumberExport { get; set; }

        public string ConditionDisplay
        {
            get { return CommonUtils.GetEnumDescription(Condition); }
        }
    }
}
