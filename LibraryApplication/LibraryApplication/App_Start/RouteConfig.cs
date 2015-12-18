using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using LibraryApplication.App_Code;
using System.Threading;
namespace LibraryApplication
{
    public class RouteConfig
    {
        private static RFIDThread thread;
        public static void RegisterRoutes(RouteCollection routes)
        {
            if (thread == null)
            {
                thread = new RFIDThread();
                Thread thready = new Thread(new ThreadStart(thread.run));
                thready.Start();
            }
            
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
