using System.Reflection;
using SpecLab.Business;
using SpecLab.Business.BusinessEnum;
using SpecLab.Business.BusinessObjects;
using SpecLab.Business.Services;
using SpecLab.FrontEnd.Models;
using SpecLab.FrontEnd.Models.SpecControl;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using log4net;

namespace SpecLab.FrontEnd.Controllers
{
    [Authorize]
    public class SpecControlController : Controller
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private StorageService _storageService = new StorageService();

        private TubeSampleService _tubeSampleService = new TubeSampleService();

        [SpecLabAuthorize(UserRightCode.R00600)]
        public ActionResult Index()
        {
            SpecControlModel model = new SpecControlModel()
            {
                EndSearchDate = DateTime.Today,
                StartSearchDate = DateTime.Today.AddDays(-SpecLabWebConstant.DefaultSearchDateDayRange), 
                TubeStatusList = WebUtils.GetTubeStatusSelectList(),
            };
            return View(model);
        }

        [SpecLabAuthorize(UserRightCode.R00601)]
        public JsonResult History(HistoryCriteria criteria)
        {
            SpecControlHistory model = new SpecControlHistory();
            try
            {
                model.SampleSpecInfo = _tubeSampleService.GetTubeDetail(criteria.TubeId);
                model.HistoryInfos = _tubeSampleService.GetTubeHistory(criteria.TubeId);
            }
            catch (BusinessException exception)
            {
                _logger.DebugFormat("BusinessException: {0}-{1}", exception.ErrorCode, exception.Message);
                model.ErrorCode = exception.ErrorCode;
                model.ErrorDescription = exception.Message;
            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("Iternal error {0}", exception));
                model.ErrorCode = ErrorCode.InternalErrorException;
                model.ErrorDescription = CommonUtils.GetEnumDescription(ErrorCode.InternalErrorException);
            }
            return Json(model);
        }

        [SpecLabAuthorize(UserRightCode.R00602)]
        public JsonResult Update(TubeUpdateModel update)
        {
            CommonModelResult model = new CommonModelResult();   
            try
            {
                if(!SessionUtils.LoginUserInfo.Rights.Contains(UserRightCode.R00603))
                {
                    var tubeDetail = _tubeSampleService.GetTubeDetail(update.TubeId);
                    if (tubeDetail != null)
                    {
                        if (tubeDetail.Status == TubeSampleStatus.Remove)
                        {
                            throw new BusinessException(ErrorCode.SampleAlreadyRemove);
                        }

                        if (update.Volume > tubeDetail.Volume)
                        {
                            throw new BusinessException(ErrorCode.NewVolumeExceedLastVolume);
                        }
                    }
                    else
                    {
                        throw new BusinessException(ErrorCode.TubeIdNotExists);
                    }
                }

                _tubeSampleService.UpdateTube(new TubeSampleService.UpdateTubeParams()
                {
                    TubeId = update.TubeId,
                    LocationNum = update.LocationNum,
                    StorageId = update.StorageId,
                    Volume = update.Volume,
                    Status = update.Status,
                    UserInput = SessionUtils.LoginUserInfo.UserId,
                    DateInput = DateTime.Now
                });
            }
            catch (BusinessException exception)
            {
                _logger.DebugFormat("BusinessException: {0}-{1}", exception.ErrorCode, exception.Message);
                model.ErrorCode = exception.ErrorCode;
                model.ErrorDescription = exception.Message;
            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("Iternal error {0}", exception));
                model.ErrorCode = ErrorCode.InternalErrorException;
                model.ErrorDescription = CommonUtils.GetEnumDescription(ErrorCode.InternalErrorException);
            }
            return Json(model);
        }

        [SpecLabAuthorize(UserRightCode.R00600)]
        public JsonResult Search(SpecControlSearchCriteria criteria)
        {
            SpecControlSearchResultModel model = new SpecControlSearchResultModel();
            try
            {
                string criteriaStorageId = (criteria.StorageId == SpecLabWebConstant.SearchAllValue
                                                ? string.Empty
                                                : criteria.StorageId);

                DateTime? criteriaFromDate = null;
                if (!string.IsNullOrEmpty(criteria.FromDate))
                {
                    criteriaFromDate = DateTime.ParseExact(criteria.FromDate,
                        CommonConstant.DateFormatDisplay, CultureInfo.InvariantCulture);
                }

                DateTime? criteriaToDate = null;
                if (!string.IsNullOrEmpty(criteria.ToDate))
                {
                    criteriaToDate = DateTime.ParseExact(criteria.ToDate,
                        CommonConstant.DateFormatDisplay, CultureInfo.InvariantCulture);
                }

                var listData = _tubeSampleService.GetListTube(new TubeSampleService.GetListTubeParams()
                {
                    SpecId = criteria.SpecId,
                    TubeId = criteria.TubeId,
                    StorageId = criteriaStorageId,
                    LocationId = criteria.LocationId,
                    FromDate = criteriaFromDate,
                    ToDate = criteriaToDate,
                    FilterStatus = criteria.FilterStatus,
                    FilterType = criteria.FilterType
                });
                if(listData.Count>0)
                {
                    int ii = 0;
                }
                model.ListData.AddRange(listData);
            }
            catch (BusinessException exception)
            {
                _logger.DebugFormat("BusinessException: {0}-{1}", exception.ErrorCode, exception.Message);
                model.ErrorCode = exception.ErrorCode;
                model.ErrorDescription = exception.Message;
            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("Iternal error {0}", exception));
                model.ErrorCode = ErrorCode.InternalErrorException;
                model.ErrorDescription = CommonUtils.GetEnumDescription(ErrorCode.InternalErrorException);
            }
            return Json(model);
        }
    }
}
