using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WMC.Web.Models;

namespace WMC.Web.Filters
{
    [Serializable]
    public class AuthorizeUserAttribute : AuthorizeAttribute
    {
        UserSessionModel userSessionModel;
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie == null)
                return false;
            if (httpContext.Session[cookie.Value] == null)
                return false;
            userSessionModel = httpContext.Session[cookie.Value] as UserSessionModel;
            return true;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException("filterContext");

            if (AuthorizeCore(filterContext.HttpContext))
                filterContext.Controller.ViewData["UserSessionModel"] = userSessionModel;
            else
                HandleUnauthorizedRequest(filterContext);
        }
    }
}