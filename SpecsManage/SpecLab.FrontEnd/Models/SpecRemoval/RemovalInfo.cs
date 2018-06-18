using SpecLab.Business;
using SpecLab.Business.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpecLab.FrontEnd.Models.SpecRemoval
{
    public class RemovalInfo : CommonModelResult
    {
        public RemovalNote RemovalNote { get; set; }
    }
}