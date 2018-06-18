using SpecLab.Business;
using SpecLab.Business.BusinessEnum;
using SpecLab.Business.BusinessObjects;
using SpecLab.Business.Services;
using SpecLab.FrontEnd.Models;
using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Reflection;
using System.Web;

namespace SpecLab.FrontEnd
{
    public static class WebUtils
    {
        private static System.Web.Script.Serialization.JavaScriptSerializer _serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

        public const string HomePageContentId = "HomePageNotice";

        public static HtmlString RenderJSON(object value)
        {
            return new HtmlString(_serializer.Serialize(value));
        }

        public static List<RightSelectItem> GetRightSelectList()
        {
            List<RightSelectItem> result = new List<RightSelectItem>();
            foreach (UserRightCode right in Enum.GetValues(typeof(UserRightCode)))
            {
                result.Add(new RightSelectItem()
                               {
                                   RightCode = right.ToString(),
                                   RightName = EnumUtils.GetEnumDescription(right)
                               });
            }
            return result;
        }

        public static List<SampleSexSelectItem> GetSampleSexSelectList()
        {
            List<SampleSexSelectItem> listResult = new List<SampleSexSelectItem>();
            foreach (SampleSex sampleSex in Enum.GetValues(typeof(SampleSex)))
            {
                listResult.Add(new SampleSexSelectItem(sampleSex));
            }

            return listResult;
        }

        public static List<SampleTypeSelectItem> GetSampleTypeSelectList()
        {
            List<SampleTypeSelectItem> listResult = new List<SampleTypeSelectItem>();
            foreach (SampleType sampleSex in Enum.GetValues(typeof(SampleType)))
            {
                listResult.Add(new SampleTypeSelectItem(sampleSex));
            }

            return listResult;
        }

        public static List<StorageSelectItem> GetStorageSelectList(List<ShortStorageInfo> valueList)
        {
            var listStorage = valueList.ConvertAll<StorageSelectItem>(ConvertStorageItem);
            listStorage.Insert(0, new StorageSelectItem
            {
                StorageId = SpecLabWebConstant.SearchAllValue,
                StorageName = SpecLabWebConstant.SearchAllDisplay,
                MaximumStorage = 0
            });
            return listStorage;
        }

        public static List<TubeStatusSelectItem> GetTubeStatusSelectList()
        {
            List<TubeStatusSelectItem> result = new List<TubeStatusSelectItem>();
            foreach (TubeSampleStatus status in Enum.GetValues(typeof(TubeSampleStatus)))
            {
                result.Add(new TubeStatusSelectItem(status));
            }
            return result;
        }

        public static StorageSelectItem ConvertStorageItem(ShortStorageInfo shortStorageInfo)
        {
            return new StorageSelectItem()
            {
                StorageId = shortStorageInfo.StorageId,
                StorageName = string.Format("{0} ({1})", shortStorageInfo.StorageId, shortStorageInfo.NumberStorage),
                MaximumStorage = shortStorageInfo.NumberStorage
            };
        }
    }
}