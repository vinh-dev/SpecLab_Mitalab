using SpecLab.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpecLab.Business.BusinessObjects;

namespace SpecLab.FrontEnd.Models.UsersControl
{
    public class UsersListModel : CommonModelResult
    {
        public List<ShortUserInfo> ListItems { get; set; }

        public UsersListModel()
        {
            ListItems = new List<ShortUserInfo>();
        }
    }
}