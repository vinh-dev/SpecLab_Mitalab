using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SpecLab.Business.BusinessEnum;

namespace SpecLab.FrontEnd
{
    public class SpecLabAuthorizeAttribute: AuthorizeAttribute
	{
        public UserRightCode RightCode { get; set; }

        public SpecLabAuthorizeAttribute(UserRightCode rightCode)
            : base()
        {
            this.RightCode = rightCode;
        }

		public override void OnAuthorization(AuthorizationContext filterContext)
		{
			if (HttpContext.Current.User.Identity.IsAuthenticated)
			{
				if (SessionUtils.LoginUserInfo == null
                    || !SessionUtils.LoginUserInfo.Rights.Contains(this.RightCode)
                    || !SessionUtils.LoginUserInfo.Rights.Contains(UserRightCode.R00100))
				{
					throw new HttpException(403, "Access Denied");
				}
			}
			else
			{
				if (filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
				{
					filterContext.Result = new JsonResult
						                       {
							                       JsonRequestBehavior = JsonRequestBehavior.AllowGet,
							                       Data = new {Message = "Authentication_Error"}
						                       };
					filterContext.HttpContext.Response.StatusCode = 500;
				}
			}
			base.OnAuthorization(filterContext);
		}
	}
}