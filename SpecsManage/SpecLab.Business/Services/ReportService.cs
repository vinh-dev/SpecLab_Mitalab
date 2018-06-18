using System.Collections.ObjectModel;
using SpecLab.Business.BusinessEnum;
using SpecLab.Business.BusinessObjects;
using SpecLab.Business.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecLab.Business.Services
{
    public class ReportService : BaseService
    {
        public class GetExportHistoryParam
        {
            public DateTime? StartDate { get; set; }

            public DateTime? EndDate { get; set; }
        }

        public List<ReportSampleStatisticInfo> GetSampleStatistics()
        {
            var command = new DatabaseCommand<string, List<ReportSampleStatisticInfo>>()
            {
                CallingInfo = null,
                ReturnValue = null
            };
            this.ProxyCalling(GetSampleStatisticsProxy, command);
            return command.ReturnValue;
        }

        private void GetSampleStatisticsProxy(SpecLabEntities _entities,
            DatabaseCommand<string, List<ReportSampleStatisticInfo>> paramCommand)
        {
            var query = _entities.viewSampleStatistics
                .Where(statistic => statistic.Status != (int)TubeSampleStatus.Remove);

            paramCommand.ReturnValue = query.ToList().ConvertAll(ConvertReportSampleStatisticInfo);
        }

        private ReportSampleStatisticInfo ConvertReportSampleStatisticInfo(viewSampleStatistic statistic)
        {
            return new ReportSampleStatisticInfo()
                       {
                           SampleSpecId = statistic.SampleSpecId,
                           TubeId = statistic.TubeId,
                           Status = (TubeSampleStatus) statistic.Status,
                           ExportNumer = statistic.NumberExport,
                           Source = statistic.Source,
                           StorageId = statistic.StorageId,
                           MaximumStorage = statistic.NumberStorage,
                           LocationNum = statistic.LocationNum,
                           FullName = statistic.PatientName,
                           SampleType = (SampleType)statistic.SampleType,
                           Sex = (SampleSex) statistic.Sex[0],
                           ImportUserId = statistic.UserInput,
                           YearOfBirth = statistic.Age,
                           Volume = statistic.Volume
                       };
        }

        public List<ReportExportHistoryInfo> GetExportHistory(GetExportHistoryParam param)
        {
            var command = new DatabaseCommand<GetExportHistoryParam, List<ReportExportHistoryInfo>>()
            {
                CallingInfo = param,
                ReturnValue = new List<ReportExportHistoryInfo>()
            };
            this.ProxyCalling(GetExportHistoryProxy, command);
            return command.ReturnValue;
        }

        private void GetExportHistoryProxy(SpecLabEntities _entities,
            DatabaseCommand<GetExportHistoryParam, List<ReportExportHistoryInfo>> paramCommand)
        {

            var historyQuery = (from history in _entities.viewExportHistories
                               select history);

            if (paramCommand.CallingInfo.StartDate != null)
            {
                historyQuery = historyQuery.Where(history => history.ExportDate >= paramCommand.CallingInfo.StartDate);
            }

            if (paramCommand.CallingInfo.EndDate != null)
            {
                var limitDate = paramCommand.CallingInfo.EndDate.GetValueOrDefault().AddDays(1);
                historyQuery = historyQuery.Where(history => history.ExportDate < limitDate);
            }

            historyQuery = historyQuery.OrderByDescending(history => history.ExportDate);

            var listResult = historyQuery.Select(history => new ReportExportHistoryInfo()
            {
                TubeId = history.TubeId,
                ExportDate = history.ExportDate,
                ExportUserId = history.ExportUserId,
                ExportReason = history.ExportReason,
                SampleType = (SampleType)history.SampleType, 
                UpdateDate = history.UpdateDate, 
                UpdateUserId = history.UpdateUserId, 
                StorageId = history.StorageId, 
                MaximumStorage = history.NumberStorage, 
                LocationNum = history.LocationNum
            });

            paramCommand.ReturnValue.AddRange(listResult);
        }

        public List<ReportStorageStatisticsInfo> GetStorageStatistics()
        {
            var command = new DatabaseCommand<string, List<ReportStorageStatisticsInfo>>()
            {
                CallingInfo = null,
                ReturnValue = null
            };
            this.ProxyCalling(GetStorageStatisticsProxy, command);
            return command.ReturnValue;
        }

        private void GetStorageStatisticsProxy(SpecLabEntities _entities,
            DatabaseCommand<string, List<ReportStorageStatisticsInfo>> paramCommand)
        {
            var query = _entities.viewStorageStatistics
                .Where(statistic => statistic.Status != (int)TubeSampleStatus.Remove)
                .OrderBy(statistic => statistic.StorageId)
                .ThenBy(statistic => statistic.LocationNum)
                .Select(statistic => new ReportStorageStatisticsInfo()
                {
                    StorageId = statistic.StorageId,
                    LocationNum = statistic.LocationNum,
                    TubeId = statistic.TubeId,
                    Status = (TubeSampleStatus)statistic.Status,
                    MaximumStorage = statistic.NumberStorage, 
                    Volume = statistic.Volume, 
                    NumberExport = statistic.NumberExport
                });

            paramCommand.ReturnValue = query.ToList();
        }
    }
}
