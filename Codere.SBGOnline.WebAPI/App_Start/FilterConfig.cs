namespace Codere.SBGOnline.WebAPI
{
    #region Using

    using Codere.SBGOnline.Common.Configuration.WebApi.Filters.Action;
    using Codere.SBGOnline.Common.WebApi.Filters.Action;
    using Codere.SBGOnline.Common.WebApi.Filters.Exception;
    using System.Web.Http;

    #endregion

    public class FilterConfig
    {
        public static void RegisterGlobalFilters(HttpConfiguration config)
        {
            config.Filters.Add(new SetCredentialsAndCultureAttribute());
        }
    }
}
