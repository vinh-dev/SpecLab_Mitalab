using SpecLab.Business;
using SpecLab.Business.BusinessEnum;
using SpecLab.Business.BusinessObjects;
using SpecLab.Business.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace SpecLab.FrontEnd
{
    public static class SessionUtils
    {
        public const string SessionKeyUserId = "LoginUserInfo";

        private static UserService _userService = new UserService();

        public static bool Exist(string key)
        {
            return HttpContext.Current.Session[key] != null;
        }

        public static T GetSession<T>(string key)
        {
            if (Exist(key))
            {
                return (T)HttpContext.Current.Session[key];
            }
            return default(T);
        }

        public static void SetSession<T>(string key, T val)
        {
            HttpContext.Current.Session[key] = val;
        }

        public static LoginUserInfo LoginUserInfo
        {
            get
            {
                if (!HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    return null;
                }
                else
                {
                    LoginUserInfo loginUserInfo = GetSession<LoginUserInfo>(SessionKeyUserId);
                    if (loginUserInfo == null)
                    {
                        var userId = HttpContext.Current.User.Identity.Name;
                        if (userId != null)
                        {
                            loginUserInfo = _userService.GetLoginUserInfo(userId);
                            SetSession<LoginUserInfo>(SessionKeyUserId, loginUserInfo);
                        }
                    }
                    else
                    {
                        var userId = HttpContext.Current.User.Identity.Name;
                        if (!loginUserInfo.UserId.Equals(userId))
                        {
                            loginUserInfo = _userService.GetLoginUserInfo(userId);
                            SetSession<LoginUserInfo>(SessionKeyUserId, loginUserInfo);
                        }
                    }

                    return loginUserInfo;
                }
            }
        }
    }
}