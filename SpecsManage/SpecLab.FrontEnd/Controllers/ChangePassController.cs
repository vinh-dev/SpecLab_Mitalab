using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using SpecLab.Business.BusinessEnum;
using SpecLab.Business.BusinessObjects;
using SpecLab.FrontEnd.Models.ChangePass;
using WebMatrix.WebData;
using SpecLab.FrontEnd.Models;
using SpecLab.Business;
using SpecLab.Business.Services;
using log4net;

namespace SpecLab.FrontEnd.Controllers
{
    [SpecLabAuthorize(UserRightCode.R00101)]
    public class ChangePassController : BaseController
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Change(ChangePassModel model)
        {
            ChangePassResult result = new ChangePassResult();
            try
            {
                result.ChangeResult = _userService.ChangePassword(new UserService.ChangePassParams()
                {
                    UserId = SessionUtils.LoginUserInfo.UserId,
                    NewPass = model.NewPassword,
                    OldPass = model.OldPassword
                });
            }
            catch (BusinessException exception)
            {
                _logger.DebugFormat("BusinessException: {0}-{1}", exception.ErrorCode, exception.Message);
                result.ErrorDescription = CommonUtils.GetEnumDescription(exception.ErrorCode);
            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("Iternal error {0}", exception));
                result.ErrorDescription = CommonUtils.GetEnumDescription(ErrorCode.InternalErrorException);
            }

            return Json(result);
        }
    }
}
