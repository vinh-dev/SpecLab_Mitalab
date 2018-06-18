using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpecLab.FrontEnd.Models.SpecRemoval
{
    public class RemovalSampleRequest
    {
        public string RemovalId { get; set; }
        public string RemovalUserId { get; set; }
        public string RemovalReason { get; set; }

        public List<string> TubeRemovalIds { get; set; }

        public RemovalSampleRequest()
        {
            TubeRemovalIds = new List<string>();
        }
    }
}