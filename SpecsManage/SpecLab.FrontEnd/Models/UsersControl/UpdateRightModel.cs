using SpecLab.Business;
using SpecLab.Business.BusinessEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpecLab.FrontEnd.Models.UsersControl
{
    public class UpdateRightModel
    {
        public string UserId { get; set; }

        public List<UserRightCode> RightCodes { get; set; }

        public UpdateRightModel()
        {
            RightCodes = new List<UserRightCode>();
        }
    }
}