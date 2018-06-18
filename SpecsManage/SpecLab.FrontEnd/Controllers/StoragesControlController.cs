using System.Reflection;
using SpecLab.Business;
using SpecLab.Business.BusinessEnum;
using SpecLab.Business.BusinessObjects;
using SpecLab.Business.Services;
using SpecLab.FrontEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using SpecLab.FrontEnd.Models.StoragesControl;
using SpecLab.FrontEnd.Models.UsersControl;
using log4net;

namespace SpecLab.FrontEnd.Controllers
{
    [SpecLabAuthorize(UserRightCode.R00300)]
    public class StoragesControlController : Controller
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private StorageService _storageService = new StorageService();

        public ActionResult Index()
        {
            StoragesControlModel model = new StoragesControlModel();
            return View(model);
        }

        [HttpPost]
        public JsonResult List( string storagefilter)
        {
            StoragesListModel result = new StoragesListModel();
            try
            {
                if(string.IsNullOrEmpty(storagefilter))
                {
                    result.StorageInfos.AddRange(_storageService.GetListStorage());
                }
                else
                {
                    result.StorageInfos.AddRange(_storageService.GetStorageInfoByID(storagefilter));
                }
            }
            catch (BusinessException exception)
            {
                _logger.DebugFormat("BusinessException: {0}-{1}", exception.ErrorCode, exception.Message);
                result.ErrorCode = exception.ErrorCode;
                result.ErrorDescription = CommonUtils.GetErrorMessage(exception);
            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("Iternal error {0}", exception));
                result.ErrorCode = ErrorCode.InternalErrorException;
                result.ErrorDescription = CommonUtils.GetEnumDescription(ErrorCode.InternalErrorException);
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult Add(ShortStorageInfo storageInfo)
        {
            CommonModelResult result = new CommonModelResult();
            try
            {
                _storageService.AddStorageInfo(storageInfo);
            }
            catch (BusinessException exception)
            {
                _logger.DebugFormat("BusinessException: {0}-{1}", exception.ErrorCode, exception.Message);
                result.ErrorCode = exception.ErrorCode;
                result.ErrorDescription = CommonUtils.GetErrorMessage(exception);
            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("Iternal error {0}", exception));
                result.ErrorCode = ErrorCode.InternalErrorException;
                result.ErrorDescription = CommonUtils.GetEnumDescription(ErrorCode.InternalErrorException);
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult Update(ShortStorageInfo storageInfo)
        {
            CommonModelResult result = new CommonModelResult();
            try
            {
                _storageService.UpdateStorageInfo(storageInfo);
            }
            catch (BusinessException exception)
            {
                _logger.DebugFormat("BusinessException: {0}-{1}", exception.ErrorCode, exception.Message);
                result.ErrorCode = exception.ErrorCode;
                result.ErrorDescription = CommonUtils.GetErrorMessage(exception);
            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("Iternal error {0}", exception));
                result.ErrorCode = ErrorCode.InternalErrorException;
                result.ErrorDescription = CommonUtils.GetEnumDescription(ErrorCode.InternalErrorException);
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult Delete(ShortStorageInfo storageInfo)
        {
            CommonModelResult result = new CommonModelResult();
            try
            {
                _storageService.DeleteStorageInfo(storageInfo.StorageId);
            }
            catch (BusinessException exception)
            {
                _logger.DebugFormat("BusinessException: {0}-{1}", exception.ErrorCode, exception.Message);
                result.ErrorCode = exception.ErrorCode;
                result.ErrorDescription = CommonUtils.GetErrorMessage(exception);
            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("Iternal error {0}", exception));
                result.ErrorCode = ErrorCode.InternalErrorException;
                result.ErrorDescription = CommonUtils.GetEnumDescription(ErrorCode.InternalErrorException);
            }

            return Json(result);
        }
        public ActionResult GetStorageInfo(string storageid)
        {
            StoragesListModel result = new StoragesListModel();
            try
            {
                result.StorageInfos.AddRange(_storageService.GetStorageInfoByID(storageid));
            }
            catch (BusinessException exception)
            {
                _logger.DebugFormat("BusinessException: {0}-{1}", exception.ErrorCode, exception.Message);
                result.ErrorCode = exception.ErrorCode;
                result.ErrorDescription = CommonUtils.GetErrorMessage(exception);
            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("Iternal error {0}", exception));
                result.ErrorCode = ErrorCode.InternalErrorException;
                result.ErrorDescription = CommonUtils.GetEnumDescription(ErrorCode.InternalErrorException);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
