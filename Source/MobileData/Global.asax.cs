using System.Web.Mvc;
using System.Web.Routing;
using iPASoftware.iRAD.Client.Infrastructure;

namespace iRAD.MobileData
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "Delivery/{action}/{id}",
                defaults: new { controller = "Delivery", action = "Index", id = UrlParameter.Optional }
            );
        }

        protected void Application_Start()
        {
            RegisterRoutes(RouteTable.Routes);
            MvvmApp.RegisterBootstrapperAsRoot<Bootstrapper>(RouteTable.Routes);
            MvvmApp.HijackRoutes(RouteTable.Routes);
        }
    }
}