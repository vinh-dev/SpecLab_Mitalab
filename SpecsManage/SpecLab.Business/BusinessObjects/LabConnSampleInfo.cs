using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpecLab.Business.BusinessEnum;
using SpecLab.Business.Database;

namespace SpecLab.Business.BusinessObjects
{
    public class LabConnSampleInfo
    {
        public string SID { get; set; }

        public string Patientname { get; set; }

        public SampleSex Sex { get; set; }

        public string SexDisplay
        {
            get { return EnumUtils.GetEnumDescription(this.Sex); }
        }

        public string LocationId { get; set; }

        public int Age { get; set; }

        public DateTime DateInput { get; set; }

        public string DateInputDisplay
        {
            get { return DateInput.ToString(CommonConstant.DateFormatDisplay); }
        }
    }
}
