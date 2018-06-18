using SpecLab.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpecLab.FrontEnd.Models.SpecImport
{
    public class SpecImportSearchModel
    {
        public DateTime EndSearchDate { get; set; }

        public DateTime StartSearchDate { get; set; }

        public string EndSearchDateString
        {
            get
            {
                return EndSearchDate.ToString(CommonConstant.DateFormatDisplay);
            }
        }

        public string StartSearchDateString
        {
            get
            {
                return StartSearchDate.ToString(CommonConstant.DateFormatDisplay);
            }
        }
    }
}