using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace TimeLive
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            ModelBinders.Binders.Add(typeof(double), new Classes.ModelBinderForDouble.DecimalModelBinder());
            ModelBinders.Binders.Add(typeof(double?), new Classes.ModelBinderForDouble.DecimalModelBinder());
            ModelBinders.Binders.Add(typeof(decimal), new Classes.ModelBinderForDecimal.DecimalModelBinder());
            ModelBinders.Binders.Add(typeof(decimal?), new Classes.ModelBinderForDecimal.DecimalModelBinder());
            App_Start.BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
