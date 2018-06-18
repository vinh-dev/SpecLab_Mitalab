using SpecLab.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpecLab.FrontEnd.Models.SpecImport
{
    public class ImportSearchCriteria
    {
        public string ImportId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}