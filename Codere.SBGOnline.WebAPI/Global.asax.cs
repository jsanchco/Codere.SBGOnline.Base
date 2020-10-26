namespace Codere.SBGOnline.WebAPI
{
    #region Using

    using Codere.SBGOnline.Common.WebApi;
    using System.Web.Http;

    #endregion

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(CodereWebApiConfig.Register);
            //GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configure(FilterConfig.RegisterGlobalFilters);
            GlobalConfiguration.Configure(AutofacConfig.Register);
        }
    }
}
