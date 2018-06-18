using SpecLab.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpecLab.FrontEnd.Models.SpecImport
{
    public class SpecImportCreateModel
    {
        public string CurrentTimeDisplay
        {
            get { return DateTime.Now.ToString(SpecLab.Business.CommonConstant.DateTimeFormatDisplay); }
        }

        public string TodayDisplay
        {
            get { return DateTime.Today.ToString(SpecLab.Business.CommonConstant.DateFormatDisplay); }
        }

        public List<SampleSexSelectItem> SexSelectItems { get; set; }

        public List<SampleTypeSelectItem> SampleTypeList { get; set; }

        public SpecImportCreateModel()
        {
            SexSelectItems = new List<SampleSexSelectItem>();
            SampleTypeList = new List<SampleTypeSelectItem>();
        }
    }
}