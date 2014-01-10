using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GettingStartedWithAngularJS.L2TAuthorizer
{
    public class MvcAuthorizer : WebAuthorizer
    {
        public ActionResult BeginAuthorization()
        {
            return new MvcOAuthActionResult(this);
        }

        public new ActionResult BeginAuthorization(Uri callback)
        {
            this.Callback = callback;
            return new GettingStartedWithAngularJS.L2TAuthorizer.MvcOAuthActionResult(this);
        }
    }

    public class MvcOAuthActionResult : ActionResult
    {
        private readonly WebAuthorizer webAuth;

        public MvcOAuthActionResult(WebAuthorizer webAuth)
        {
            this.webAuth = webAuth;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            webAuth.PerformRedirect = authUrl =>
            {
                HttpContext.Current.Response.Redirect(authUrl);
            };

            Uri callback =
                webAuth.Callback == null ?
                    HttpContext.Current.Request.Url :
                    webAuth.Callback;

            webAuth.BeginAuthorization(callback);
        }
    }
}