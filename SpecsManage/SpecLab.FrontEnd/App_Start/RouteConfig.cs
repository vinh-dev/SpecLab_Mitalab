using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using SpecLab.Business;

namespace SpecLab.FrontEnd
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "ReportsExportHistory",
                url: "Reports/ExportHistory/{startDate}-{endDate}",
                defaults: new
                {
                    controller = "Reports",
                    action = "ExportHistory",
                    //startDate = DateTime.Today.AddDays(-30).ToString(CommonConstant.UniversalDateFormat),
                    //endDate = DateTime.Today.ToString(CommonConstant.UniversalDateFormat)
                }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = SpecLabWebConstant.DefaultController, action = SpecLabWebConstant.DefaultControllerDefaultAction, id = UrlParameter.Optional }
            );
        }
    }
}