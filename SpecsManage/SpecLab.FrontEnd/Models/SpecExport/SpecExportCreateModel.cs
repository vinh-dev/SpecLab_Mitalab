using SpecLab.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpecLab.Business.BusinessObjects;

namespace SpecLab.FrontEnd.Models.SpecExport
{
    public class SpecExportCreateModel
    {
        public string ExportId { get; set; }

        public string ExportDateDisplay
        {
            get { return DateTime.Now.ToString(CommonConstant.DateTimeFormatDisplay); }
        }

        public ShortUserInfo CurrentUser { get; set; }

        public List<ShortUserInfo> UserList { get; set; }

        public List<int> StatusAllowList { get; set; }

        public bool HasRightOverride { get; set; }

        public SpecExportCreateModel()
        {
            this.UserList = new List<ShortUserInfo>();
            this.StatusAllowList = new List<int>();
        }
    }
}