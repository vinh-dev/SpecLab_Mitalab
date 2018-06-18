using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpecLab.Business.BusinessEnum;

namespace SpecLab.Business.BusinessObjects
{
    public class TubeSampleSearchDataItem
    {
        public string SpecId { get; set; }

        public string TubeId { get; set; }

        public DateTime InputDate { get; set; }

        public string InputDateDisplay
        {
            get
            {
                return this.InputDate.ToString(CommonConstant.DateTimeFormatDisplay);
            }
        }

        public string StorageId { get; set; }

        public int LocationNum { get; set; }

        public TubeSampleStatus Status { get; set; }
        public TubeSampleType Type { get; set; }

        public CalculatedCondition Condition
        {
            get { return CommonUtils.GetCalculatedCondition(Status,Type, Volume, NumberExport); }
        }

        public string ConditionDisplay
        {
            get { return CommonUtils.GetEnumDescription(Condition); }
        }

        public string StatusDisplay {
            get
            {
                return CommonUtils.GetEnumDescription(this.Status);
            }
        }

        public double Volume { get; set; }

        public string VolumeDisplay { get { return Volume.ToString("0"); } }

        public int NumberExport { get; set; }
    }
}
