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
    [SpecLabAuthorize(UserRightCode.R00100)]
    public class HomeController : BaseController
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ActionResult Index()
        {
            return View();
        }
    }
}
