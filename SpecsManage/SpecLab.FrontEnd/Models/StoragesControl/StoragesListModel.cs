using SpecLab.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpecLab.Business.BusinessObjects;

namespace SpecLab.FrontEnd.Models.StoragesControl
{
    public class StoragesListModel : CommonModelResult
    {
        public List<ShortStorageInfo> StorageInfos { get; set; }

        public StoragesListModel()
        {
            StorageInfos = new List<ShortStorageInfo>();
        }
    }
}