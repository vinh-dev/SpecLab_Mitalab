//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SpecLab.Business.Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class ExportDetail
    {
        public long ExportDetailId { get; set; }
        public string ExportId { get; set; }
        public string TubeId { get; set; }
        public int Status { get; set; }
        public double Volume { get; set; }
        public string StorageId { get; set; }
        public int LocationNum { get; set; }
        public int NumberExport { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public Nullable<int> TubeType { get; set; }
    
        public virtual Export Export { get; set; }
        public virtual TubeSample TubeSample { get; set; }
    }
}
