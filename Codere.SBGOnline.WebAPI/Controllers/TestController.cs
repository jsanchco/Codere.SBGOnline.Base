namespace Codere.SBGOnline.WebAPI.Controllers
{
    #region Using

    using Codere.SBGOnline.Common;
    using Codere.SBGOnline.Common.Logging.Interfaces;
    using Codere.SBGOnline.Common.Logging.TraceSource;
    using Codere.SBGOnline.Common.Model.Authentication;
    using Codere.SBGOnline.Services.Interfaces;
    using System;
    using System.Net;
    using System.Web.Http;    

    #endregion

    public class TestController : ApiController
    {
        private readonly ILogger _logger;
        private readonly IServiceTest _serviceTest;

        public TestController(ILogger logger, IServiceTest serviceTest)
        {
            _logger = logger ??
                throw new ArgumentNullException(nameof(logger));

            _serviceTest = serviceTest ??
                throw new ArgumentNullException(nameof(serviceTest));
        }

        [HttpGet]
        public IHttpActionResult GetContact()
        {
            try
            {
                var principal = RequestContext.Principal as CoderePrincipal;
                if (principal == null || 
                    principal.ContactId == null || 
                    principal.ContactId == Guid.Empty)
                {
                    //_logger.WriteInformation("Session not connected");
                    CodereTrace.Instance().Trace("Session not connected");
                    return Content(HttpStatusCode.BadRequest, "Session not connected");
                }

                var contact = _serviceTest.Contacts(principal.Identity.Name);
                if (contact == null)
                {
                    //_logger.WriteInformation($"Contact [{principal.Identity.Name}] -> Not Found");
                    CodereTrace.Instance().Trace($"Contact [{principal.Identity.Name}] -> Not Found");
                    return NotFound();
                }

                //_logger.WriteInformation($"Contact [{principal.Identity.Name}] -> Found");
                CodereTrace.Instance().Trace($"Contact [{principal.Identity.Name}] -> Found");
                return Ok(contact);
            }
            catch (Exception ex)
            {
                //_logger.WriteError("Error in TestController -> GetContact", ex);
                CodereTrace.Instance().LogException(1111, ex, "Error in TestController -> GetContact");
                return Content(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
