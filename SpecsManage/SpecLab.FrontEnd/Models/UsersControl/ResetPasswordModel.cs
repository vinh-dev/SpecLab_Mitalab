using SpecLab.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpecLab.FrontEnd.Models.UsersControl
{
    public class ResetPasswordModel
    {
        public string UserId { get; set; }

        public string NewPassword { get; set; }
    }
}