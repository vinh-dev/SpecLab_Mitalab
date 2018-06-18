using SpecLab.Business.BusinessEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecLab.Business.BusinessObjects
{
    public class SampleSpecInfo
    {
        public string SampleSpecId { get; set; }

        public string PatientName { get; set; }

        public SampleSex Sex { get; set; }

        public string SexDisplay 
        { 
            get { return EnumUtils.GetEnumDescription(Sex); }
        }

        public int YearOfBirth { get; set; }

        public string LocationId { get; set; }

        public string UserInput { get; set; }

        public DateTime DateInput { get; set; }

        public string DateInputDisplay 
        { 
            get { return DateInput.ToString(CommonConstant.DateTimeFormatDisplay); }
        }

        public List<TubeSampleSpecInfo> TubeSampleSpecs { get; set; }

        public SampleSpecInfo()
        {
            this.TubeSampleSpecs = new List<TubeSampleSpecInfo>();
        }
    }
}
