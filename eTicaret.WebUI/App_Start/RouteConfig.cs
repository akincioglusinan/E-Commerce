using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace eTicaret.WebUI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            //routes.MapRoute(
            //    name: "Default2",
            //    url: "{Siparis}/{KargoyaVerilenSiparisler}",
            //    defaults: new { controller = "Siparis", action = "OnayBekleyenSiparisler" }
            //);

    //        routes.MapRoute("KargoyaVerilenSiparisler" , "{Siparis}/{KargoyaVerilenSiparisler}",
    //new { controller = "Siparis", action = "OnayBekleyenSiparisler", id = UrlParameter.Optional });
        }
    }
}
