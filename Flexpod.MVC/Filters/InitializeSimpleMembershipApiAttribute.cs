using System;
using System.Threading;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Flexpod.MVC.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class InitializeSimpleMembershipApiAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            // Ensure ASP.NET Simple Membership is initialized only once per app start
            LazyInitializer.EnsureInitialized(ref InitializeSimpleMembershipSync._initializer,
                ref InitializeSimpleMembershipSync._isInitialized,
                ref InitializeSimpleMembershipSync._initializerLock);
        }
    }
}
