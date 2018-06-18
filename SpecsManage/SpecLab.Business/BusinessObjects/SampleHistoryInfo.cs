using SpecLab.Business.BusinessEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpecLab.Business.Database;

namespace SpecLab.Business.BusinessObjects
{
    public class SampleHistoryInfo
    {
        public long HistoryId { get; set; }
        public string TubeId { get; set; }
        public System.DateTime HistoryDate { get; set; }

        public string HistoryDateDisplay
        {
            get
            {
                return HistoryDate.ToString(CommonConstant.DateTimeFormatDisplay);
            }
        }
        public HistoryAction Action { get; set; }

        public string ActionDisplay 
        { 
            get { return EnumUtils.GetEnumDescription(Action); } 
        }
        public string UserId { get; set; }
        public double Volume { get; set; }
        public string VolumeDisplay { get { return Volume.ToString("0"); } }
        public TubeSampleStatus Status { get; set; }
        public TubeSampleType Type { get; set; }
        public string StatusDisplay { get { return EnumUtils.GetEnumDescription(Status); } }
        public string StorageId { get; set; }
        public int LocationNum { get; set; }
        public string LocationNumDisplay { get { return LocationNum.ToString(); } }
        public string Description { get; set; }

        public string StorageDisplay
        {
            get
            {
                return CommonUtils.GetStorageDisplay(StorageId, LocationNum);
            }
        }

        public CalculatedCondition Condition
        {
            get { return CommonUtils.GetCalculatedCondition(Status,Type, Volume, NumberExport); }
        }

        public string ConditionDisplay
        {
            get { return CommonUtils.GetEnumDescription(Condition); }
        }

        public int NumberExport { get; set; }
    }
}
