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
using WebMatrix.WebData;
using SpecLab.FrontEnd.Models;
using SpecLab.Business;
using SpecLab.Business.Services;
using log4net;

namespace SpecLab.FrontEnd.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    LoginUserInfo userInfo = this._userService.Login(model.UserName, model.Password);

                    if (userInfo.Rights.Contains(UserRightCode.R00100))
                    {
                        FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        throw new BusinessException(ErrorCode.NotAllowToLogin);
                    }
                }
            }
            catch (BusinessException exception)
            {
                _logger.DebugFormat("BusinessException: {0}-{1}", exception.ErrorCode, exception.Message);
                ModelState.AddModelError("", CommonUtils.GetEnumDescription(exception.ErrorCode));
            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("Iternal error {0}", exception));
                ModelState.AddModelError("", CommonUtils.GetEnumDescription(ErrorCode.InternalErrorException));
            }
            return View(model);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(SpecLabWebConstant.DefaultControllerDefaultAction, SpecLabWebConstant.DefaultController);
            }
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            SessionUtils.SetSession<LoginUserInfo>(SessionUtils.SessionKeyUserId, null);
            return RedirectToAction(SpecLabWebConstant.DefaultControllerDefaultAction, SpecLabWebConstant.DefaultController);
        }
    }
}
