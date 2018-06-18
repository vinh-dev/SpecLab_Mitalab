using System.Reflection;
using SpecLab.Business;
using SpecLab.Business.Services;
using SpecLab.FrontEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using SpecLab.FrontEnd.Models.SpecControl;
using SpecLab.FrontEnd.Models.UsersControl;
using SpecLab.Business.BusinessObjects;
using SpecLab.Business.BusinessEnum;
using log4net;

namespace SpecLab.FrontEnd.Controllers
{
    [SpecLabAuthorize(UserRightCode.R00200)]
    public class UsersControlController : BaseController
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ActionResult Index()
        {
            UsersControlModel model = new UsersControlModel();
            return View(model);
        }

        [HttpPost]
        public JsonResult List()
        {
            UsersListModel result = new UsersListModel();
            try
            {
                result.ListItems.AddRange(_userService.GetAllShortUserInfo());
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
        public JsonResult Update(UpdateUserModel model)
        {
            CommonModelResult result = new CommonModelResult();
            try
            {
                _userService.UpdateUser(new LoginUserInfo()
                                            {
                                                UserId = model.UserId,
                                                FullName = model.NewName
                                            });
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
        public JsonResult Add(AddUserModel model)
        {
            CommonModelResult result = new CommonModelResult();
            try
            {
                _userService.CreateNewUser(new LoginUserInfo()
                                               {
                                                   UserId = model.UserId, 
                                                   FullName = model.FullName, 
                                                   Status = UserStatus.Enable,
                                                   Rights = new List<UserRightCode>(CommonUtils.GetListDefaultUserRightCode())
                                               }, CommonConstant.DefaultPassword);
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
        [SpecLabAuthorize(UserRightCode.R00201)]
        public JsonResult ResetPass(ResetPasswordModel model)
        {
            CommonModelResult result = new CommonModelResult();
            try
            {
                _userService.ResetPassword(new UserService.ResetPassParams()
                                               {
                                                   UserId = model.UserId,
                                                   NewPass = model.NewPassword
                                               });
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
        public JsonResult Delete(DeleteUserModel model)
        {
            CommonModelResult result = new CommonModelResult();
            try
            {
                _userService.DeleteUser(model.UserId);
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

        private string ConvertUserRightCode(UserRightCode rightCode)
        {
            return rightCode.ToString();
        }

        [HttpPost]
        [SpecLabAuthorize(UserRightCode.R00202)]
        public JsonResult ListRight(ListRightCriteria criteria)
        {
            ListRightResult model = new ListRightResult();
            try
            {
                var userInfo = _userService.GetLoginUserInfo(criteria.UserId);
                model.UserId = criteria.UserId;
                model.RightCodes.AddRange(userInfo.Rights.ConvertAll(ConvertUserRightCode));
            }
            catch (BusinessException exception)
            {
                _logger.DebugFormat("BusinessException: {0}-{1}", exception.ErrorCode, exception.Message);
                model.ErrorCode = exception.ErrorCode;
                model.ErrorDescription = CommonUtils.GetErrorMessage(exception);
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
        [SpecLabAuthorize(UserRightCode.R00202)]
        public JsonResult UpdateRight(UpdateRightModel updateRightModel)
        {
            CommonModelResult model = new CommonModelResult();
            try
            {
                var updateParams = new UserService.UpdateRightParams()
                {
                    UserId = updateRightModel.UserId,
                };
                updateParams.UpdateRights.AddRange(updateRightModel.RightCodes);

                _userService.UpdateUserRights(updateParams);
            }
            catch (BusinessException exception)
            {
                _logger.DebugFormat("BusinessException: {0}-{1}", exception.ErrorCode, exception.Message);
                model.ErrorCode = exception.ErrorCode;
                model.ErrorDescription = CommonUtils.GetErrorMessage(exception);
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
