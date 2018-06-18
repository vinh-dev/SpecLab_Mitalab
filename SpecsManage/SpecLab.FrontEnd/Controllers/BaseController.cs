using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SpecLab.Business.BusinessObjects;
using SpecLab.Business.Services;
using log4net;

namespace SpecLab.FrontEnd.Controllers
{
    public abstract class BaseController : Controller
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected UserService _userService = new UserService();
    }
}