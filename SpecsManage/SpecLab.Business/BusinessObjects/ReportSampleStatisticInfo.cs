using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpecLab.Business.BusinessEnum;

namespace SpecLab.Business.BusinessObjects
{
    public class ReportSampleStatisticInfo
    {
        public string SampleSpecId { get; set; }

        public string TubeId { get; set; }

        public SampleType SampleType { get; set; }

        public string SampleTypeDisplay
        {
            get { return EnumUtils.GetEnumDescription(SampleType); }
        }

        public string FullName { get; set; }

        public SampleSex Sex { get; set; }

        public string SexDisplay { get { return Sex!=0?EnumUtils.GetEnumDescription(Sex):"?"; } }

        public int YearOfBirth { get; set; }

        public string YearOfBirthDisplay
        {
            get { return string.Format("{0}", YearOfBirth); }
        }

        public string Source { get; set; }

        public string StorageId { get; set; }

        public int MaximumStorage { get; set; }

        public int LocationNum { get; set; }

        public string StorageDisplay
        {
            get
            {
                return string.Format("{0} ({1}) - {2}", StorageId, MaximumStorage, LocationNum);
            }
        }

        public string LocationNumDisplay
        {
            get { return string.Format("{0}", LocationNum); }
        }

        public int ExportNumer { get; set; }

        public string ExportNumerDisplay
        {
            get { return string.Format("{0}", ExportNumer); }
        }

        public TubeSampleStatus Status { get; set; }
        public TubeSampleType Type { get; set; }

        public string StatusDisplay { get { return EnumUtils.GetEnumDescription(Status); } }

        public CalculatedCondition Condition
        {
            get { return CommonUtils.GetCalculatedCondition(Status,Type, Volume, ExportNumer); }
        }

        public double Volume { get; set; }

        public string ConditionDisplay
        {
            get { return CommonUtils.GetEnumDescription(Condition); }
        }

        public string ImportUserId { get; set; }
    }
}
