using SpecLab.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpecLab.Business.BusinessEnum;

namespace SpecLab.FrontEnd.Models.SpecImport
{
    public class LabConnSearchInfo
    {
        public string SID { get; set; }

        public string PatientName { get; set; }

        public string DateSearch { get; set; }

        public string Sequence { get; set; }
    }
}