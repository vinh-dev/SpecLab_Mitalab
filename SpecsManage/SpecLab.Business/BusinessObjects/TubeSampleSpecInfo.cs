using SpecLab.Business.BusinessEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecLab.Business.BusinessObjects
{
    public class TubeSampleSpecInfo
    {
        public string SampleSpecId { get; set; }

        public string TubeId { get; set; }

        public TubeSampleStatus Status { get; set; }
        public TubeSampleType Type { get; set; }

        public string StatusDisplay { get { return EnumUtils.GetEnumDescription(Status); } }

        public double Volume { get; set; }

        public string StorageId { get; set; }

        public int LocationNum { get; set; }

        public DateTime DateInput { get; set; }

        public string DateInputDisplay { get { return DateInput.ToString(CommonConstant.DateTimeFormatDisplay); } }

        public SampleType SampleType { get; set; }

        public string SampleTypeDisplay
        {
            get { return EnumUtils.GetEnumDescription(SampleType); }
        }
    }
}
