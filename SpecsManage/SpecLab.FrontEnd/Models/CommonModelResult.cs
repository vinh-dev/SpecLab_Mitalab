using SpecLab.Business.BusinessEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpecLab.FrontEnd.Models
{
    public class CommonModelResult
    {
        public ErrorCode ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
    }
}