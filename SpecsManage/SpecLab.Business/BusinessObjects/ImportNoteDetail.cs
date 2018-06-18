using SpecLab.Business.BusinessEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecLab.Business.BusinessObjects
{
    public class ImportNoteDetail
    {
        public long ImportDetailId { get; set; }
        public string ImportId { get; set; }
        public string TubeId { get; set; }
        public string SampleSpecId { get; set; }
        public TubeSampleStatus Status { get; set; }
        public TubeSampleType Type { get; set; }
        public string StatusDisplay { get { return EnumUtils.GetEnumDescription(Status); } }
        public double Volume { get; set; }
        public string VolumeDisplay { get { return Volume.ToString(CommonConstant.NumberRoundingDisplay); } }
        public string StorageId { get; set; }
        public int LocationNum { get; set; }

        // for report
        public string LocationNumDisplay { get { return this.LocationNum.ToString(); } }

        public SampleType SampleType { get; set; }

        public string SampleTypeDisplay
        {
            get { return EnumUtils.GetEnumDescription(SampleType); }
        }

        public string StorageDisplay
        {
            get
            {
                return CommonUtils.GetStorageDisplay(StorageId, LocationNum);
            }
        }

        public DateTime ImportDate { get; set; }
        public string ImportDateDisplay { get { return ImportDate.ToString(CommonConstant.DateFormatDisplay); } }

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
