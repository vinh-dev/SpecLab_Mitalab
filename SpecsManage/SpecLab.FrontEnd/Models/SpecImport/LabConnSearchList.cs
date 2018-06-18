using SpecLab.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpecLab.Business.BusinessEnum;
using SpecLab.Business.BusinessObjects;

namespace SpecLab.FrontEnd.Models.SpecImport
{
    public class LabConnSearchList : CommonModelResult
    {
        public List<LabConnSampleInfo> SampleInfos { get; set; }

        public LabConnSearchList()
        {
            this.SampleInfos = new List<LabConnSampleInfo>();
        }
    }
}