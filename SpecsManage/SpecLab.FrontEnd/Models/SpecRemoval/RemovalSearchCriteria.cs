﻿using SpecLab.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpecLab.FrontEnd.Models.SpecRemoval
{
    public class RemovalSearchCriteria
    {
        public string RemovalId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}