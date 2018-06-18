using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpecLab.FrontEnd.Models.ContentControl
{
    public class ContentDetailModel : CommonModelResult
    {
        public string ContentId { get; set; }
        public string ContentText { get; set; }
    }
}