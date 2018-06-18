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
using SpecLab.FrontEnd.Models.ContentControl;

namespace SpecLab.FrontEnd.Controllers
{
    [SpecLabAuthorize(UserRightCode.R00900)]
    public class ContentControlController : Controller
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private ContentService _contentService = new ContentService();

        public ActionResult Index()
        {
            ContentControlModel model = new ContentControlModel();
            return View(model);
        }

        public JsonResult GetContent(ContentSearchCriteria criteria)
        {
            ContentDetailModel model = new ContentDetailModel();
            try
            {
                var contentInfo = _contentService.GetContentInfo(criteria.ContentId);
                if (contentInfo != null)
                {
                    model.ContentId = contentInfo.ContentId;
                    model.ContentText = contentInfo.ContentText;
                }
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

        [HttpPost]
        public JsonResult SaveContent(ContentInfo contentInfo)
        {
            CommonModelResult result = new CommonModelResult();
            try
            {
                _contentService.SaveContentInfo(contentInfo);
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
    }
}
