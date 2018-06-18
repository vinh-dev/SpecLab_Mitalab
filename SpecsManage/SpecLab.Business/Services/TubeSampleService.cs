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
    public class TubeSampleService : BaseService
    {
        public class GetListTubeParams
        {
            public DateTime? FromDate { get; set; }
            public DateTime? ToDate { get; set; }
            public string SpecId { get; set; }
            public string TubeId { get; set; }
            public string StorageId { get; set; }
            public int? LocationId { get; set; }

            public List<TubeSampleStatus> FilterStatus { get; set; }
            public List<TubeSampleType> FilterType { get; set; }

            public GetListTubeParams()
            {
                FilterStatus = new List<TubeSampleStatus>();
                FilterType = new List<TubeSampleType>();
            }
        }

        public class UpdateTubeParams
        {
            public string TubeId { get; set; }
            public TubeSampleStatus Status { get; set; }
            public TubeSampleType Type { get; set; }
            public double Volume { get; set; }
            public string StorageId { get; set; }
            public int LocationNum { get; set; }
            public string UserInput { get; set; }
            public DateTime DateInput { get; set; }
        }

        public class GetExportListParams
        {
            public string ExportId { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
        }

        public class GetImportListParams
        {
            public string ImportId { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
        }

        public class GetRemovalListParams
        {
            public string RemovalId { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
        }

        public class ExportSampleParam
        {
            public string ExportId { get; set; }
            public string ExportUserId { get; set; }
            public string ExportForUserId { get; set; }
            public DateTime ExportDate { get; set; }
            public string ExportReason { get; set; }
            public List<string> TubeExportIds { get; set; }

            public ExportSampleParam()
            {
                TubeExportIds = new List<string>();
            }
        }

        public class RemovalSampleParam
        {
            public string RemovalId { get; set; }
            public string RemovalUserId { get; set; }
            public string RemovalReason { get; set; }
            public DateTime RemovalDate { get; set; }

            public List<string> TubeRemovalIds { get; set; }

            public RemovalSampleParam()
            {
                TubeRemovalIds = new List<string>();
            }
        }

        public TubeSampleSpecInfo GetTubeDetail(string tubeId)
        {
            var command = new DatabaseCommand<string, TubeSampleSpecInfo>()
            {
                CallingInfo = tubeId,
                ReturnValue = null
            };
            this.ProxyCalling(GetTubeDetailProxy, command);
            return command.ReturnValue;
        }

        private void GetTubeDetailProxy(SpecLabEntities _entities,
            DatabaseCommand<string, TubeSampleSpecInfo> paramCommand)
        {
            var summaryInfo = (from tube in _entities.TubeSamples
                               join sample in _entities.SampleSpecs on tube.SampleSpecId equals sample.SampleSpecId
                               where tube.TubeId == paramCommand.CallingInfo
                               select new TubeSampleSpecInfo()
                               {
                                   DateInput = sample.DateInput,
                                   LocationNum = tube.LocationNum,
                                   StorageId = tube.StorageId,
                                   Status = (TubeSampleStatus)tube.Status,
                                   Type = (TubeSampleType)tube.TubeType,
                                   TubeId = tube.TubeId,
                                   Volume = tube.Volume,
                                   SampleSpecId = tube.SampleSpecId
                               }).FirstOrDefault();

            paramCommand.ReturnValue = summaryInfo;
        }

        public List<SampleHistoryInfo> GetTubeHistory(string tubeId)
        {
            var command = new DatabaseCommand<string, List<SampleHistoryInfo>>()
            {
                CallingInfo = tubeId,
                ReturnValue = new List<SampleHistoryInfo>()
            };
            this.ProxyCalling(GetTubeHistoryProxy, command);
            return command.ReturnValue;
        }

        private void GetTubeHistoryProxy(SpecLabEntities _entities,
            DatabaseCommand<string, List<SampleHistoryInfo>> paramCommand)
        {
            var historyQuery = (from history in _entities.SampleHistories
                                where history.TubeId == paramCommand.CallingInfo
                                select new SampleHistoryInfo()
                                {
                                    HistoryId = history.HistoryId,
                                    TubeId = history.TubeId,
                                    HistoryDate = history.HistoryDate,
                                    Action = (HistoryAction)history.Action,
                                    UserId = history.UserId,
                                    Description = history.Description,
                                    LocationNum = history.LocationNum,
                                    StorageId = history.StorageId,
                                    Status = (TubeSampleStatus)history.Status,
                                     Type = (TubeSampleType)history.TubeType,
                                    Volume = history.Volume,
                                    NumberExport = history.NumberExport,
                                });
            historyQuery = historyQuery.OrderByDescending(his => his.HistoryDate);
            paramCommand.ReturnValue.AddRange(historyQuery);
        }

        public List<TubeSampleSearchDataItem> GetListTube(GetListTubeParams tubeParams)
        {
            var command = new DatabaseCommand<GetListTubeParams, List<TubeSampleSearchDataItem>>()
            {
                CallingInfo = tubeParams,
                ReturnValue = new List<TubeSampleSearchDataItem>()
            };
            this.ProxyCalling(GetListTubeProxy, command);
            return command.ReturnValue;
        }

        private void GetListTubeProxy(SpecLabEntities _entities,
            DatabaseCommand<GetListTubeParams, List<TubeSampleSearchDataItem>> paramCommand)
        {
            var tubeQuery = (from tube in _entities.TubeSamples
                             join sample in _entities.SampleSpecs on tube.SampleSpecId equals sample.SampleSpecId
                             //where tube.Status != (int)TubeSampleStatus.Remove 
                             select new { tube, sample });

            if (paramCommand.CallingInfo.FromDate != null)
            {
                tubeQuery = tubeQuery.Where(t => t.sample.DateInput >= paramCommand.CallingInfo.FromDate);
            }

            if (paramCommand.CallingInfo.ToDate != null)
            {
                var limitDate = paramCommand.CallingInfo.ToDate.GetValueOrDefault().AddDays(1);
                tubeQuery = tubeQuery.Where(t => t.sample.DateInput < limitDate);
            }

            if (!string.IsNullOrEmpty(paramCommand.CallingInfo.SpecId))
            {
                tubeQuery = tubeQuery.Where(t => t.tube.SampleSpecId.Contains(
                    paramCommand.CallingInfo.SpecId));
            }

            if (!string.IsNullOrEmpty(paramCommand.CallingInfo.TubeId))
            {
                tubeQuery = tubeQuery.Where(t => t.tube.TubeId.Contains(
                    paramCommand.CallingInfo.TubeId));
            }

            if (!string.IsNullOrEmpty(paramCommand.CallingInfo.StorageId))
            {
                tubeQuery = tubeQuery.Where(t => t.tube.StorageId == paramCommand.CallingInfo.StorageId);
            }

            if (paramCommand.CallingInfo.LocationId != null)
            {
                tubeQuery = tubeQuery.Where(t => t.tube.LocationNum == paramCommand.CallingInfo.LocationId);
            }

            if (paramCommand.CallingInfo.FilterStatus.Count > 0)
            {
                List<int> statusValueList = paramCommand.CallingInfo.FilterStatus.Select(t => (int)t).ToList();
                tubeQuery = tubeQuery.Where(t => statusValueList.Contains(t.tube.Status));
            }


            tubeQuery = tubeQuery.OrderByDescending(t => t.sample.DateInput);

            var listResult = tubeQuery.Select(t => new TubeSampleSearchDataItem()
            {
                InputDate = t.sample.DateInput,
                SpecId = t.tube.SampleSpecId,
                TubeId = t.tube.TubeId,
                LocationNum = t.tube.LocationNum,
                StorageId = t.tube.StorageId,
                Status = (TubeSampleStatus)t.tube.Status,
                Type = (TubeSampleType)t.tube.TubeType,
                Volume = t.tube.Volume,
                NumberExport = t.tube.NumberExport
            });

            paramCommand.ReturnValue.AddRange(listResult);
        }

        private void ImportSampleProxy(SpecLabEntities _entities,
            DatabaseCommand<SampleSpecInfo, bool> paramCommand)
        {
            if (paramCommand.CallingInfo.TubeSampleSpecs.Count == 0)
            {
                throw new BusinessException(ErrorCode.ImportNoTube);
            }

            var sampleDbItem = (from sample in _entities.SampleSpecs
                                where sample.SampleSpecId.Equals(
                                    paramCommand.CallingInfo.SampleSpecId, StringComparison.OrdinalIgnoreCase)
                                select sample).FirstOrDefault();

            if (sampleDbItem != null)
            {
                throw new BusinessException(ErrorCode.SampleSpecIdExists);
            }

            var listTubeId = paramCommand.CallingInfo.TubeSampleSpecs.Select(t => t.TubeId);
            var countTubeIdDuplicate = (from tube in _entities.TubeSamples
                                        where listTubeId.Contains(tube.TubeId)
                                        select tube.TubeId).Count();

            if (countTubeIdDuplicate > 0)
            {
                throw new BusinessException(ErrorCode.TubeIdExists);
            }

            for (int i = 0; i < paramCommand.CallingInfo.TubeSampleSpecs.Count - 1; i++)
            {
                if (paramCommand.CallingInfo.TubeSampleSpecs[i].Volume <= 0)
                {
                    throw new BusinessException(ErrorCode.ImportTubeEmptyVolume);
                }

                for (int j = i + 1; j < paramCommand.CallingInfo.TubeSampleSpecs.Count; j++)
                {
                    if (paramCommand.CallingInfo.TubeSampleSpecs[i].StorageId.Equals(
                        paramCommand.CallingInfo.TubeSampleSpecs[j].StorageId)
                        && paramCommand.CallingInfo.TubeSampleSpecs[i].LocationNum ==
                            paramCommand.CallingInfo.TubeSampleSpecs[j].LocationNum)
                    {
                        throw new BusinessException(ErrorCode.DuplicateLocationId);
                    }
                }
            }

            for (int i = 0; i < paramCommand.CallingInfo.TubeSampleSpecs.Count; i++)
            {
                var tubeSampleSpecs = paramCommand.CallingInfo.TubeSampleSpecs[i];
                ValidateStorageId(_entities, tubeSampleSpecs.StorageId, tubeSampleSpecs.LocationNum);

                var innerQuery = (from t in _entities.TubeSamples
                                  where t.StorageId == tubeSampleSpecs.StorageId
                                        && t.LocationNum == tubeSampleSpecs.LocationNum
                                        && t.Status != (int)TubeSampleStatus.Remove
                                  select t.TubeId);

                if (innerQuery.Any())
                {
                    throw new BusinessException(ErrorCode.StorageLocationUsed, tubeSampleSpecs.StorageId, tubeSampleSpecs.LocationNum);
                }
            }

            var sexValue = ((char)paramCommand.CallingInfo.Sex).ToString();
            var sampleSpec = new SampleSpec()
            {
                SampleSpecId = paramCommand.CallingInfo.SampleSpecId,
                Age = paramCommand.CallingInfo.YearOfBirth,
                PatientName = paramCommand.CallingInfo.PatientName,
                Sex = sexValue,
                Source = paramCommand.CallingInfo.LocationId,
                SampleNumber = paramCommand.CallingInfo.TubeSampleSpecs.Count,
                UserInput = paramCommand.CallingInfo.UserInput,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                DateInput = paramCommand.CallingInfo.DateInput,
            };

            var importDbItem = new Import()
            {
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                ImportDate = paramCommand.CallingInfo.DateInput,
                ImportUserId = paramCommand.CallingInfo.UserInput,
                SampleNumber = paramCommand.CallingInfo.TubeSampleSpecs.Count,
                ImportId = paramCommand.CallingInfo.SampleSpecId
            };

            for (int i = 0; i < paramCommand.CallingInfo.TubeSampleSpecs.Count; i++)
            {
                var tubeSample = paramCommand.CallingInfo.TubeSampleSpecs[i];
                var tubeDbItem = new TubeSample()
                {
                    SampleSpecId = paramCommand.CallingInfo.SampleSpecId,
                    Status = (int)TubeSampleStatus.Good,
                    TubeType = (int)TubeSampleType.InStorage,
                    Volume = tubeSample.Volume,
                    StorageId = tubeSample.StorageId,
                    LocationNum = tubeSample.LocationNum,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    TubeId = tubeSample.TubeId,
                    TubeOrder = i,
                    SampleType = (int)tubeSample.SampleType
                };
                tubeDbItem.SampleHistories.Add(new SampleHistory()
                {
                    HistoryDate = DateTime.Now,
                    Action = (int)HistoryAction.Import,
                    UserId = paramCommand.CallingInfo.UserInput,
                    Status = (int)TubeSampleStatus.Good,
                    TubeType = (int)TubeSampleType.InStorage,
                    Volume = tubeSample.Volume,
                    StorageId = tubeSample.StorageId,
                    LocationNum = tubeSample.LocationNum,
                    TubeId = tubeSample.TubeId,
                    Description = ""
                });

                sampleSpec.TubeSamples.Add(tubeDbItem);

                importDbItem.ImportDetails.Add(new ImportDetail()
                {
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Status = (int)TubeSampleStatus.Good,
                    TubeType = (int)TubeSampleType.InStorage,
                    StorageId = tubeSample.StorageId,
                    LocationNum = tubeSample.LocationNum,
                    TubeId = tubeSample.TubeId,
                    Volume = tubeSample.Volume,
                });
            }

            _entities.SampleSpecs.Add(sampleSpec);
            _entities.Imports.Add(importDbItem);
            _entities.SaveChanges();
        }

        public ExportNote GetExportInfo(string exportId)
        {
            var command = new DatabaseCommand<string, ExportNote>()
            {
                CallingInfo = exportId,
                ReturnValue = null
            };
            this.ProxyCalling(GetExportInfoProxy, command);
            return command.ReturnValue;
        }

        public ImportNote GetImportInfo(string importId)
        {
            var command = new DatabaseCommand<string, ImportNote>()
            {
                CallingInfo = importId,
                ReturnValue = null
            };
            this.ProxyCalling(GetImportInfoProxy, command);
            return command.ReturnValue;
        }

        public RemovalNote GetRemovalInfo(string removalId)
        {
            var command = new DatabaseCommand<string, RemovalNote>()
            {
                CallingInfo = removalId,
                ReturnValue = null
            };
            this.ProxyCalling(GetRemovalInfoProxy, command);
            return command.ReturnValue;
        }

        private void GetRemovalInfoProxy(SpecLabEntities _entities,
            DatabaseCommand<string, RemovalNote> paramCommand)
        {
            var result = (from removal in _entities.Removals
                          where removal.RemovalId.Equals(
                              paramCommand.CallingInfo, StringComparison.OrdinalIgnoreCase)
                          select new RemovalNote()
                          {
                              RemovalDate = removal.RemovalDate,
                              RemovalId = removal.RemovalId,
                              RemovalUserId = removal.RemovalUserId,
                              RemovalReason = removal.RemovalReason,
                          }).FirstOrDefault();

            if (result == null)
            {
                throw new BusinessException(ErrorCode.RemovalIdNotExists);
            }

            var listRemovalDetail = (from removalDetail in _entities.RemovalDetails
                                     join tube in _entities.TubeSamples on removalDetail.TubeId equals tube.TubeId
                                     join spec in _entities.SampleSpecs on tube.SampleSpecId equals spec.SampleSpecId
                                     where removalDetail.RemovalId.Equals(
                                        paramCommand.CallingInfo, StringComparison.OrdinalIgnoreCase)
                                     select new RemovalNoteDetail()
                                     {
                                         RemovalId = removalDetail.RemovalId,
                                         TubeId = removalDetail.TubeId,
                                         RemovalDetailId = removalDetail.RemovalDetailId,
                                         Status = (TubeSampleStatus)removalDetail.Status,
                                         Type = (TubeSampleType)removalDetail.TubeType,
                                         Volume = removalDetail.Volume,
                                         LocationNum = removalDetail.LocationNum,
                                         StorageId = removalDetail.StorageId,
                                         SampleSpecId = tube.SampleSpecId,
                                         InputDate = spec.DateInput,
                                         NumberExport = removalDetail.NumberExport
                                     }).ToList();
            result.RemovalNoteDetails.AddRange(listRemovalDetail);

            paramCommand.ReturnValue = result;
        }

        private void GetImportInfoProxy(SpecLabEntities _entities,
            DatabaseCommand<string, ImportNote> paramCommand)
        {
            var result = (from import in _entities.Imports
                          where import.ImportId.Equals(
                              paramCommand.CallingInfo, StringComparison.OrdinalIgnoreCase)
                          select new ImportNote()
                          {
                              ImportDate = import.ImportDate,
                              ImportId = import.ImportId,
                              ImportUserId = import.ImportUserId,
                          }).FirstOrDefault();

            if (result == null)
            {
                throw new BusinessException(ErrorCode.ExportIdNotExists);
            }

            var listExportDetail = (from importDetail in _entities.ImportDetails
                                    join import in _entities.Imports on importDetail.ImportId equals import.ImportId
                                    join tube in _entities.TubeSamples on importDetail.TubeId equals tube.TubeId
                                    where importDetail.ImportId.Equals(
                                        paramCommand.CallingInfo, StringComparison.OrdinalIgnoreCase)
                                    select new ImportNoteDetail()
                                    {
                                        ImportId = importDetail.ImportId,
                                        TubeId = importDetail.TubeId,
                                        ImportDetailId = importDetail.ImportDetailId,
                                        Status = (TubeSampleStatus)importDetail.Status,
                                        Type = (TubeSampleType)importDetail.TubeType,
                                        Volume = importDetail.Volume,
                                        LocationNum = importDetail.LocationNum,
                                        StorageId = importDetail.StorageId,
                                        SampleSpecId = tube.SampleSpecId,
                                        SampleType = (SampleType)tube.SampleType,
                                        ImportDate = import.ImportDate,
                                        NumberExport = importDetail.NumberExport
                                    }).ToList();
            result.ImportNoteDetails.AddRange(listExportDetail);

            paramCommand.ReturnValue = result;
        }

        private void GetExportInfoProxy(SpecLabEntities _entities,
            DatabaseCommand<string, ExportNote> paramCommand)
        {
            var result = (from export in _entities.Exports
                          where export.ExportId.Equals(
                              paramCommand.CallingInfo, StringComparison.OrdinalIgnoreCase)
                          select new ExportNote()
                          {
                              ExportDate = export.ExportDate,
                              ExportId = export.ExportId,
                              ExportUserId = export.ExportUserId,
                              ExportForUserId = export.ExportToUserId,
                              ExportReason = export.ExportReason
                          }).FirstOrDefault();

            if (result == null)
            {
                throw new BusinessException(ErrorCode.ExportIdNotExists);
            }

            var listExportDetail = (from exportDetail in _entities.ExportDetails
                                    join tube in _entities.TubeSamples on exportDetail.TubeId equals tube.TubeId
                                    where exportDetail.ExportId.Equals(
                                        paramCommand.CallingInfo, StringComparison.OrdinalIgnoreCase)
                                    select new ExportNoteDetail()
                                    {
                                        ExportId = exportDetail.ExportId,
                                        TubeId = exportDetail.TubeId,
                                        ExportDetailId = exportDetail.ExportDetailId,
                                        Status = (TubeSampleStatus)exportDetail.Status,
                                        Type = (TubeSampleType)exportDetail.TubeType,
                                        Volume = exportDetail.Volume,
                                        LocationNum = exportDetail.LocationNum,
                                        StorageId = exportDetail.StorageId,
                                        SampleSpecId = tube.SampleSpecId,
                                        NumberExport = exportDetail.NumberExport
                                    }).ToList();
            result.ExportNoteDetails.AddRange(listExportDetail);

            paramCommand.ReturnValue = result;
        }

        public void ImportSample(SampleSpecInfo sampleSpec)
        {
            var command = new DatabaseCommand<SampleSpecInfo, bool>()
            {
                CallingInfo = sampleSpec,
                ReturnValue = false
            };
            this.ProxyCalling(ImportSampleProxy, command);
        }

        private void GetSampleSpecProxy(SpecLabEntities _entities,
            DatabaseCommand<string, SampleSpecInfo> paramCommand)
        {
            var sampleDbItem = (from sample in _entities.SampleSpecs
                                where sample.SampleSpecId.Equals(
                                    paramCommand.CallingInfo, StringComparison.OrdinalIgnoreCase)
                                select sample).FirstOrDefault();

            if (sampleDbItem == null)
            {
                throw new BusinessException(ErrorCode.SampleSpecIdNotExists);
            }

            SampleSpecInfo info = new SampleSpecInfo()
            {
                LocationId = sampleDbItem.Source,
                PatientName = sampleDbItem.PatientName,
                SampleSpecId = sampleDbItem.SampleSpecId,
                YearOfBirth = sampleDbItem.Age,
                Sex = (SampleSex)sampleDbItem.Sex[0],
                UserInput = sampleDbItem.UserInput,
                DateInput = sampleDbItem.DateInput
            };

            var tubeListDbItem = (from tube in _entities.TubeSamples
                                  where tube.SampleSpecId.Equals(
                                      paramCommand.CallingInfo, StringComparison.OrdinalIgnoreCase)
                                  select new TubeSampleSpecInfo()
                                  {
                                      Volume = tube.Volume,
                                      LocationNum = tube.LocationNum,
                                      SampleSpecId = tube.SampleSpecId,
                                      StorageId = tube.StorageId,
                                      TubeId = tube.TubeId,
                                      Status = (TubeSampleStatus)tube.Status,
                                      DateInput = sampleDbItem.DateInput,
                                      Type = (TubeSampleType)tube.TubeType,
                                      SampleType = (SampleType)tube.SampleType,
                                  }).ToList();

            info.TubeSampleSpecs.AddRange(tubeListDbItem);

            paramCommand.ReturnValue = info;
        }

        public SampleSpecInfo GetSampleSpec(string sampleSpecId)
        {
            var command = new DatabaseCommand<string, SampleSpecInfo>()
            {
                CallingInfo = sampleSpecId,
            };
            this.ProxyCalling(GetSampleSpecProxy, command);
            return command.ReturnValue;
        }

        private void GetImportHistoryProxy(SpecLabEntities _entities,
            DatabaseCommand<string, List<SampleHistoryInfo>> paramCommand)
        {
            var sampleDbItem = (from sample in _entities.SampleSpecs
                                where sample.SampleSpecId.Equals(
                                    paramCommand.CallingInfo, StringComparison.OrdinalIgnoreCase)
                                select sample).FirstOrDefault();

            if (sampleDbItem == null)
            {
                throw new BusinessException(ErrorCode.SampleSpecIdNotExists);
            }

            var historyListDbItem = (from tube in _entities.TubeSamples
                                     join history in _entities.SampleHistories on tube.TubeId equals history.TubeId
                                     where tube.SampleSpecId.Equals(paramCommand.CallingInfo, StringComparison.OrdinalIgnoreCase)
                                         && history.Action == (int)HistoryAction.Import
                                     select new SampleHistoryInfo()
                                     {
                                         HistoryId = history.HistoryId,
                                         TubeId = history.TubeId,
                                         HistoryDate = history.HistoryDate,
                                         Action = (HistoryAction)history.Action,
                                         UserId = history.UserId,
                                         Description = history.Description,
                                         LocationNum = history.LocationNum,
                                         StorageId = history.StorageId,
                                         Status = (TubeSampleStatus)history.Status,
                                         Type = (TubeSampleType)history.TubeType,
                                         Volume = history.Volume
                                     });

            List<SampleHistoryInfo> listData = new List<SampleHistoryInfo>();
            listData.AddRange(historyListDbItem);

            paramCommand.ReturnValue = listData;
        }

        public List<SampleHistoryInfo> GetImportHistory(string sampleSpecId)
        {
            var command = new DatabaseCommand<string, List<SampleHistoryInfo>>()
            {
                CallingInfo = sampleSpecId,
            };
            this.ProxyCalling(GetImportHistoryProxy, command);
            return command.ReturnValue;
        }

        public void UpdateTube(UpdateTubeParams updateTube)
        {
            var command = new DatabaseCommand<UpdateTubeParams, bool>()
            {
                CallingInfo = updateTube,
            };
            this.ProxyCalling(UpdateTubeProxy, command);
        }

        /// <summary>
        /// kiểm tra storage id 
        /// </summary>
        /// <param name="_entities"></param>
        /// <param name="storageId"></param>
        private void ValidateStorageId(SpecLabEntities _entities, string storageId, int locationNum)
        {
            var query = (from storage in _entities.Storages
                         where storage.StorageId.Equals(storageId, StringComparison.OrdinalIgnoreCase)
                         select storage.NumberStorage);

            if (!query.Any())
            {
                throw new BusinessException(ErrorCode.StorageIdNotExists, storageId);
            }
            else
            {
                var numberStorage = query.FirstOrDefault();
                if (locationNum > numberStorage)
                {
                    throw new BusinessException(ErrorCode.StorageLocationOutOfBound, storageId, numberStorage);
                }
            }
        }

        private void UpdateTubeProxy(SpecLabEntities _entities,
            DatabaseCommand<UpdateTubeParams, bool> paramCommand)
        {
            var sampleDbItem = (from tube in _entities.TubeSamples
                                where tube.TubeId.Equals(
                                        paramCommand.CallingInfo.TubeId, StringComparison.OrdinalIgnoreCase)
                                select tube).FirstOrDefault();

            if (sampleDbItem == null)
            {
                throw new BusinessException(ErrorCode.TubeIdNotExists);
            }

            // kiểm tra chuyển trạng thái trực tiếp
            TubeSampleType currentType = (TubeSampleType)sampleDbItem.TubeType;
            TubeSampleStatus currentStatus = (TubeSampleStatus)sampleDbItem.Status;

            if (paramCommand.CallingInfo.Type == TubeSampleType.InStorage)
            {
                if (paramCommand.CallingInfo.Status == TubeSampleStatus.Remove)
                {
                    if (currentStatus != TubeSampleStatus.Remove)
                    {
                        throw new BusinessException(ErrorCode.TubeUpdateStatusRemove);
                    }
                }
            }
            else if (paramCommand.CallingInfo.Type == TubeSampleType.InUse)
            {
                if (currentType != TubeSampleType.InUse)
                {
                    throw new BusinessException(ErrorCode.TubeUpdateStatusInUse);
                }
            }

            ValidateStorageId(_entities, paramCommand.CallingInfo.StorageId, paramCommand.CallingInfo.LocationNum);

            // check duplicate location
            var checkLocationQuery = (from tube in _entities.TubeSamples
                                      where tube.StorageId == paramCommand.CallingInfo.StorageId
                                            && tube.LocationNum == paramCommand.CallingInfo.LocationNum
                                            && tube.Status != (int)TubeSampleStatus.Remove
                                            && !tube.TubeId.Equals(
                                                paramCommand.CallingInfo.TubeId, StringComparison.OrdinalIgnoreCase)
                                      select tube.TubeId);

            if (checkLocationQuery.Any())
            {
                throw new BusinessException(ErrorCode.StorageLocationUsed,
                    paramCommand.CallingInfo.StorageId, paramCommand.CallingInfo.LocationNum);
            }

            // check location storage > maximum storage
            checkLocationQuery = (from storage in _entities.Storages
                                  where storage.StorageId == paramCommand.CallingInfo.StorageId
                                        && storage.NumberStorage < paramCommand.CallingInfo.LocationNum
                                  select storage.StorageId);

            if (checkLocationQuery.Any())
            {
                throw new BusinessException(ErrorCode.StorageLocationOutOfBound,
                    paramCommand.CallingInfo.StorageId, paramCommand.CallingInfo.LocationNum);
            }

            sampleDbItem.Volume = paramCommand.CallingInfo.Volume;
            sampleDbItem.StorageId = paramCommand.CallingInfo.StorageId;
            sampleDbItem.LocationNum = paramCommand.CallingInfo.LocationNum;
            sampleDbItem.Status = (int)paramCommand.CallingInfo.Status;
            sampleDbItem.TubeType = (int)paramCommand.CallingInfo.Type;
            sampleDbItem.UpdateDate = DateTime.Now;

            sampleDbItem.SampleHistories.Add(new SampleHistory()
            {
                HistoryDate = paramCommand.CallingInfo.DateInput,
                Action = (int)HistoryAction.Update,
                UserId = paramCommand.CallingInfo.UserInput,
                Status = (int)paramCommand.CallingInfo.Status,
                TubeType = (int)paramCommand.CallingInfo.Type,
                Volume = paramCommand.CallingInfo.Volume,
                StorageId = paramCommand.CallingInfo.StorageId,
                LocationNum = paramCommand.CallingInfo.LocationNum,
                TubeId = paramCommand.CallingInfo.TubeId,
                Description = ""
            });

            _entities.SaveChanges();
        }

        public List<ExportNoteShortData> GetExportList(GetExportListParams criteria)
        {
            var command = new DatabaseCommand<GetExportListParams, List<ExportNoteShortData>>()
            {
                CallingInfo = criteria,
                ReturnValue = new List<ExportNoteShortData>()
            };
            this.ProxyCalling(GetExportListProxy, command);
            return command.ReturnValue;
        }

        public List<ImportNoteShortData> GetImportList(GetImportListParams criteria)
        {
            var command = new DatabaseCommand<GetImportListParams, List<ImportNoteShortData>>()
            {
                CallingInfo = criteria,
                ReturnValue = new List<ImportNoteShortData>()
            };
            this.ProxyCalling(GetImportListProxy, command);
            return command.ReturnValue;
        }

        public List<RemovalNoteShortData> GetRemovalList(GetRemovalListParams criteria)
        {
            var command = new DatabaseCommand<GetRemovalListParams, List<RemovalNoteShortData>>()
            {
                CallingInfo = criteria,
                ReturnValue = new List<RemovalNoteShortData>()
            };
            this.ProxyCalling(GetRemovalListProxy, command);
            return command.ReturnValue;
        }

        private void GetRemovalListProxy(SpecLabEntities _entities,
            DatabaseCommand<GetRemovalListParams, List<RemovalNoteShortData>> paramCommand)
        {
            var removalQuery = (from import in _entities.Removals
                                select import);

            if (!string.IsNullOrEmpty(paramCommand.CallingInfo.RemovalId))
            {
                removalQuery = removalQuery.Where(e => e.RemovalId.StartsWith(paramCommand.CallingInfo.RemovalId));
            }

            if (paramCommand.CallingInfo.StartDate != null)
            {
                removalQuery = removalQuery.Where(e => e.RemovalDate >= paramCommand.CallingInfo.StartDate);
            }

            if (paramCommand.CallingInfo.EndDate != null)
            {
                var limitDate = paramCommand.CallingInfo.EndDate.GetValueOrDefault().AddDays(1);
                removalQuery = removalQuery.Where(e => e.RemovalDate < limitDate);
            }

            removalQuery = removalQuery.OrderByDescending(t => t.RemovalDate);

            var listResult = removalQuery.Select(t => new RemovalNoteShortData()
            {
                RemovalId = t.RemovalId,
                RemovalUserId = t.RemovalUserId,
                RemovalDate = t.RemovalDate,
                RemovalReason = t.RemovalReason,
                NumberRemoval = t.SampleNumber
            });

            paramCommand.ReturnValue.AddRange(listResult);
        }

        private void GetImportListProxy(SpecLabEntities _entities,
            DatabaseCommand<GetImportListParams, List<ImportNoteShortData>> paramCommand)
        {
            var importQuery = (from import in _entities.Imports
                               select import);

            if (!string.IsNullOrEmpty(paramCommand.CallingInfo.ImportId))
            {
                importQuery = importQuery.Where(e => e.ImportId.StartsWith(paramCommand.CallingInfo.ImportId));
            }

            if (paramCommand.CallingInfo.StartDate != null)
            {
                importQuery = importQuery.Where(e => e.ImportDate >= paramCommand.CallingInfo.StartDate);
            }

            if (paramCommand.CallingInfo.EndDate != null)
            {
                var limitDate = paramCommand.CallingInfo.EndDate.GetValueOrDefault().AddDays(1);
                importQuery = importQuery.Where(e => e.ImportDate < limitDate);
            }

            var listResult = importQuery.Select(t => new ImportNoteShortData()
            {
                ImportId = t.ImportId,
                ImportUserId = t.ImportUserId,
                ImportDate = t.ImportDate,
                NumberImport = t.SampleNumber
            });

            listResult = listResult.OrderByDescending(t => t.ImportDate);

            paramCommand.ReturnValue.AddRange(listResult);
        }

        private void GetExportListProxy(SpecLabEntities _entities,
            DatabaseCommand<GetExportListParams, List<ExportNoteShortData>> paramCommand)
        {
            var exportQuery = (from export in _entities.Exports
                               select export);

            if (!string.IsNullOrEmpty(paramCommand.CallingInfo.ExportId))
            {
                exportQuery = exportQuery.Where(e => e.ExportId.StartsWith(paramCommand.CallingInfo.ExportId));
            }

            if (paramCommand.CallingInfo.StartDate != null)
            {
                exportQuery = exportQuery.Where(e => e.ExportDate >= paramCommand.CallingInfo.StartDate);
            }

            if (paramCommand.CallingInfo.EndDate != null)
            {
                var limitDate = paramCommand.CallingInfo.EndDate.GetValueOrDefault().AddDays(1);
                exportQuery = exportQuery.Where(e => e.ExportDate < limitDate);
            }

            var listResult = exportQuery.Select(t => new ExportNoteShortData()
            {
                ExportId = t.ExportId,
                ExportUserId = t.ExportUserId,
                ExportForUserId = t.ExportToUserId,
                ExportDate = t.ExportDate,
                NumberExport = t.SampleNumber,
                ExportReason = t.ExportReason
            });

            listResult = listResult.OrderByDescending(t => t.ExportDate);

            paramCommand.ReturnValue.AddRange(listResult);
        }

        public void ExportSamples(ExportSampleParam exportNote)
        {
            var command = new DatabaseCommand<ExportSampleParam, bool>()
            {
                CallingInfo = exportNote,
                ReturnValue = false
            };
            this.ProxyCalling(ExportSamplesProxy, command);
        }

        public void RemovalSamples(RemovalSampleParam removalNote)
        {
            var command = new DatabaseCommand<RemovalSampleParam, bool>()
            {
                CallingInfo = removalNote,
                ReturnValue = false
            };
            this.ProxyCalling(RemovalSamplesProxy, command);
        }

        private void RemovalSamplesProxy(SpecLabEntities _entities,
            DatabaseCommand<RemovalSampleParam, bool> paramCommand)
        {
            var checkExists = (from removal in _entities.Removals
                               where removal.RemovalId == paramCommand.CallingInfo.RemovalId
                               select removal.RemovalId);

            if (checkExists.Any())
            {
                throw new BusinessException(ErrorCode.RemovalIdExists);
            }

            var removalDbItem = new Removal()
            {
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                RemovalDate = paramCommand.CallingInfo.RemovalDate,
                RemovalId = paramCommand.CallingInfo.RemovalId,
                RemovalUserId = paramCommand.CallingInfo.RemovalUserId,
                RemovalReason = paramCommand.CallingInfo.RemovalReason,
                SampleNumber = paramCommand.CallingInfo.TubeRemovalIds.Count
            };

            foreach (var tubeId in paramCommand.CallingInfo.TubeRemovalIds)
            {
                var tubeDbItem = (from tube in _entities.TubeSamples
                                  where tube.TubeId == tubeId
                                  select tube).FirstOrDefault();

                if (tubeDbItem == null)
                {
                    throw new BusinessException(ErrorCode.TubeIdNotExists);
                }

                removalDbItem.RemovalDetails.Add(new RemovalDetail()
                {
                    RemovalId = paramCommand.CallingInfo.RemovalId,
                    TubeId = tubeId,
                    Status = tubeDbItem.Status,
                    TubeType = tubeDbItem.TubeType,
                    StorageId = tubeDbItem.StorageId,
                    LocationNum = tubeDbItem.LocationNum,
                    Volume = tubeDbItem.Volume,
                    NumberExport = tubeDbItem.NumberExport,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now
                });

                tubeDbItem.Status = (int)TubeSampleStatus.Remove;
                tubeDbItem.SampleHistories.Add(new SampleHistory()
                {
                    HistoryDate = DateTime.Now,
                    Action = (int)HistoryAction.Remove,
                    UserId = paramCommand.CallingInfo.RemovalUserId,
                    Status = tubeDbItem.Status,
                    TubeType = tubeDbItem.TubeType,
                    Volume = tubeDbItem.Volume,
                    StorageId = tubeDbItem.StorageId,
                    LocationNum = tubeDbItem.LocationNum,
                    TubeId = tubeDbItem.TubeId,
                    Description = "",
                    NumberExport = tubeDbItem.NumberExport,
                });
            }

            _entities.Removals.Add(removalDbItem);
            _entities.SaveChanges();
        }

        private void ExportSamplesProxy(SpecLabEntities _entities,
            DatabaseCommand<ExportSampleParam, bool> paramCommand)
        {
            var checkExists = (from export in _entities.Exports
                               where export.ExportId == paramCommand.CallingInfo.ExportId
                               select export.ExportId);

            if (checkExists.Any())
            {
                throw new BusinessException(ErrorCode.ExportIdExists);
            }

            var exportDbItem = new Export()
            {
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                ExportDate = paramCommand.CallingInfo.ExportDate,
                ExportId = paramCommand.CallingInfo.ExportId,
                ExportToUserId = paramCommand.CallingInfo.ExportForUserId,
                ExportUserId = paramCommand.CallingInfo.ExportUserId,
                ExportReason = paramCommand.CallingInfo.ExportReason,
                SampleNumber = paramCommand.CallingInfo.TubeExportIds.Count
            };

            foreach (var tubeId in paramCommand.CallingInfo.TubeExportIds)
            {
                var tubeDbItem = (from tube in _entities.TubeSamples
                                  where tube.TubeId == tubeId
                                  select tube).FirstOrDefault();

                if (tubeDbItem == null)
                {
                    throw new BusinessException(ErrorCode.TubeIdNotExists);
                }

                exportDbItem.ExportDetails.Add(new ExportDetail()
                {
                    ExportId = paramCommand.CallingInfo.ExportId,
                    TubeId = tubeId,
                    Status = tubeDbItem.Status,
                    TubeType = tubeDbItem.TubeType,
                    StorageId = tubeDbItem.StorageId,
                    LocationNum = tubeDbItem.LocationNum,
                    Volume = tubeDbItem.Volume,
                    NumberExport = tubeDbItem.NumberExport,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now
                });

                //tubeDbItem.Status = (int)TubeSampleStatus.InUse;
                tubeDbItem.TubeType = (int)TubeSampleType.InUse;
                tubeDbItem.NumberExport += 1;
                tubeDbItem.SampleHistories.Add(new SampleHistory()
                {
                    HistoryDate = DateTime.Now,
                    Action = (int)HistoryAction.Export,
                    UserId = paramCommand.CallingInfo.ExportUserId,
                    Status = tubeDbItem.Status,
                    TubeType = tubeDbItem.TubeType,
                    Volume = tubeDbItem.Volume,
                    StorageId = tubeDbItem.StorageId,
                    LocationNum = tubeDbItem.LocationNum,
                    TubeId = tubeDbItem.TubeId,
                    Description = string.Format("Xuất cho {0} với mục đích {1}.",
                        paramCommand.CallingInfo.ExportForUserId,
                        paramCommand.CallingInfo.ExportReason),
                    NumberExport = tubeDbItem.NumberExport,
                });
            }

            _entities.Exports.Add(exportDbItem);
            _entities.SaveChanges();
        }
    }
}
