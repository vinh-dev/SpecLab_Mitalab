using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpecLab.Business.BusinessEnum;

namespace SpecLab.Business.BusinessObjects
{
    public class ReportExportHistoryInfo
    {

        public string TubeId { get; set; }

        public SampleType SampleType { get; set; }

        public string SampleTypeDisplay
        {
            get { return EnumUtils.GetEnumDescription(SampleType); }
        }

        public string ExportUserId { get; set; }

        public DateTime ExportDate { get; set; }

        public string ExportDateDisplay { get { return ExportDate.ToString(CommonConstant.DateTimeFormatDisplay); } }

        public string ExportReason { get; set; }

        public string UpdateUserId { get; set; }

        public DateTime? UpdateDate { get; set; }

        public string UpdateDateDisplay
        {
            get
            {
                if (UpdateDate == null)
                {
                    return "";
                }

                return UpdateDate.Value.ToString(CommonConstant.DateTimeFormatDisplay);
            }
        }

        public string StorageId { get; set; }

        public int MaximumStorage { get; set; }

        public int LocationNum { get; set; }

        public string StorageDisplay
        {
            get
            {
                return CommonUtils.GetStorageDisplay(StorageId, LocationNum);
            }
        }
    }
}
