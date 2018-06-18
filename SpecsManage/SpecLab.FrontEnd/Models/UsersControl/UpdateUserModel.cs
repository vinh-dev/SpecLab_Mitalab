using SpecLab.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpecLab.FrontEnd.Models.UsersControl
{
    public class UpdateUserModel
    {
        public string UserId { get; set; }

        public string NewName { get; set; }
    }
}