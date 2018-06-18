using SpecLab.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpecLab.Business.BusinessObjects;

namespace SpecLab.FrontEnd.Models.SpecRemoval
{
    public class SpecRemovalCreateModel
    {
        public string RemovalId { get; set; }

        public string RemovalDateDisplay
        {
            get { return DateTime.Now.ToString(CommonConstant.DateTimeFormatDisplay); }
        }

        public ShortUserInfo CurrentUser { get; set; }

        public List<ShortUserInfo> UserList { get; set; }

        public List<int> StatusAllowList { get; set; }

        public SpecRemovalCreateModel()
        {
            this.UserList = new List<ShortUserInfo>();
            this.StatusAllowList = new List<int>();
        }
    }
}