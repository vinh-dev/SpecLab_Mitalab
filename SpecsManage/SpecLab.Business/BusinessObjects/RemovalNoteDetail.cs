using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpecLab.Business.BusinessEnum;

namespace SpecLab.Business.BusinessObjects
{
    public class RemovalNoteDetail
    {
        public long RemovalDetailId { get; set; }
        public string RemovalId { get; set; }
        public string TubeId { get; set; }
        public string SampleSpecId { get; set; }
        public TubeSampleStatus Status { get; set; }
        public TubeSampleType Type { get; set; }
        public string StatusDisplay { get { return EnumUtils.GetEnumDescription(Status); } }
        public double Volume { get; set; }
        public string VolumeDisplay { get { return Volume.ToString(CommonConstant.NumberRoundingDisplay); } }
        public string StorageId { get; set; }
        public int LocationNum { get; set; }
        public DateTime InputDate { get; set; }
        public string InputDateDisplay
        {
            get { return InputDate.ToString(CommonConstant.DateFormatDisplay); }
        }

        // for report
        public string LocationNumDisplay { get { return this.LocationNum.ToString(); } }

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
