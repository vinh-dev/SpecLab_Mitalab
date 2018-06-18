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
    public class StorageService : BaseService
    {
        private void GetListStorageIdProxy(SpecLabEntities _entities,
            DatabaseCommand<object, List<ShortStorageInfo>> paramCommand)
        {
            var listStorageId = (from u in _entities.Storages
                                 select new ShortStorageInfo()
                                            {
                                                StorageId = u.StorageId,
                                                NumberStorage = u.NumberStorage,
                                                NumRows = u.NumRows,
                                                NumColumn=u.NumColumn
                                 }).ToList();

            paramCommand.ReturnValue = listStorageId;
        }

        private string ConvertShortStorageInfoToString(ShortStorageInfo info)
        {
            return info.ToString();
        }

        public List<string> GetListStorageId()
        {
            var command = new DatabaseCommand<object, List<ShortStorageInfo>>();
            this.ProxyCalling(GetListStorageIdProxy, command);
            return command.ReturnValue.ConvertAll(ConvertShortStorageInfoToString);
        }

        public List<ShortStorageInfo> GetListStorage()
        {
            var command = new DatabaseCommand<object, List<ShortStorageInfo>>();
            this.ProxyCalling(GetListStorageIdProxy, command);
            return command.ReturnValue;
        }
        public List<ShortStorageInfo> GetStorageInfoByID(string _storageid)
        {
            ShortStorageInfo _ShortStorageInfo = new ShortStorageInfo();
            var command = new DatabaseCommand<object, List<ShortStorageInfo>>();
            this.ProxyCalling(GetListStorageIdProxy, command);
            return command.ReturnValue.Where(s => s.StorageId.Contains(_storageid)).ToList();
        }
        private void AddStorageInfoProxy(SpecLabEntities _entities,
            DatabaseCommand<ShortStorageInfo, object> paramCommand)
        {
            var existStorage = (from storage in _entities.Storages
                                where storage.StorageId.Equals(paramCommand.CallingInfo.StorageId, StringComparison.OrdinalIgnoreCase)
                                select storage).FirstOrDefault();

            if(existStorage != null)
            {
                throw new BusinessException(ErrorCode.StorageIdExists);
            }
            else
            {
                _entities.Storages.Add(new Storage()
                                           {
                                               UpdateDate = DateTime.Now,
                                               CreateDate = DateTime.Now,
                                               NumberStorage = paramCommand.CallingInfo.NumberStorage,
                    NumColumn = paramCommand.CallingInfo.NumColumn,
                    NumRows = paramCommand.CallingInfo.NumRows,
                    StorageId = paramCommand.CallingInfo.StorageId
                                           });

                _entities.SaveChanges();
            }
        }

        public void AddStorageInfo(ShortStorageInfo storageInfo)
        {
            var command = new DatabaseCommand<ShortStorageInfo, object>()
                              {
                                  CallingInfo = storageInfo
                              };
            this.ProxyCalling(AddStorageInfoProxy, command);
        }

        private void UpdateStorageInfoProxy(SpecLabEntities _entities,
            DatabaseCommand<ShortStorageInfo, object> paramCommand)
        {
            var existStorage = (from storage in _entities.Storages
                                where storage.StorageId.Equals(paramCommand.CallingInfo.StorageId, StringComparison.OrdinalIgnoreCase)
                                select storage).FirstOrDefault();

            if (existStorage == null)
            {
                throw new BusinessException(ErrorCode.StorageIdNotExists, paramCommand.CallingInfo.StorageId);
            }
            else
            {
                existStorage.NumberStorage = paramCommand.CallingInfo.NumberStorage;
                existStorage.NumRows = paramCommand.CallingInfo.NumRows;
                existStorage.NumColumn = paramCommand.CallingInfo.NumColumn;
                _entities.SaveChanges();
            }
        }

        public void UpdateStorageInfo(ShortStorageInfo storageInfo)
        {
            var command = new DatabaseCommand<ShortStorageInfo, object>()
            {
                CallingInfo = storageInfo
            };
            this.ProxyCalling(UpdateStorageInfoProxy, command);
        }

        private void DeleteStorageInfoProxy(SpecLabEntities _entities,
            DatabaseCommand<string, object> paramCommand)
        {
            var existStorage = (from storage in _entities.Storages
                                where storage.StorageId.Equals(paramCommand.CallingInfo, StringComparison.OrdinalIgnoreCase)
                                select storage).FirstOrDefault();

            if (existStorage == null)
            {
                throw new BusinessException(ErrorCode.StorageIdNotExists, paramCommand.CallingInfo);
            }
            else
            {
                var checkStorageInUse = (from exists in _entities.TubeSamples
                                         where exists.StorageId == paramCommand.CallingInfo
                                         select exists.StorageId);
                if(checkStorageInUse.Any())
                {
                    throw new BusinessException(ErrorCode.StorageIdAlreadyInUse);
                }

                checkStorageInUse = (from exists in _entities.SampleHistories
                                     where exists.StorageId == paramCommand.CallingInfo
                                     select exists.StorageId);
                if (checkStorageInUse.Any())
                {
                    throw new BusinessException(ErrorCode.StorageIdAlreadyInUse);
                }

                _entities.Storages.Remove(existStorage);
                _entities.SaveChanges();
            }
        }

        public void DeleteStorageInfo(string storageId)
        {
            var command = new DatabaseCommand<string, object>()
            {
                CallingInfo = storageId
            };
            this.ProxyCalling(DeleteStorageInfoProxy, command);
        }
    }
}
