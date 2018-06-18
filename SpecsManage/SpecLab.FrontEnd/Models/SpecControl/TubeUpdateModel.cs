using SpecLab.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpecLab.Business.BusinessEnum;

namespace SpecLab.FrontEnd.Models.SpecControl
{
    public class TubeUpdateModel
    {
        public string TubeId { get; set; }
        public TubeSampleStatus Status { get; set; }
        public double Volume { get; set; }
        public string StorageId { get; set; }
        public int LocationNum { get; set; }
    }
}