using SpecLab.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpecLab.Business.BusinessEnum;

namespace SpecLab.FrontEnd.Models.UsersControl
{
    public class ListRightResult : CommonModelResult
    {
        public string UserId { get; set; }

        public List<string> RightCodes { get; set; }

        public ListRightResult()
        {
            RightCodes = new List<string>();
        }
    }
}