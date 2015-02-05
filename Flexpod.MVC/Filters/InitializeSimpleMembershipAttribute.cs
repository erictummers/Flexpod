using System;
using System.Threading;
using System.Web.Mvc;

namespace Flexpod.MVC.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class InitializeSimpleMembershipAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Ensure ASP.NET Simple Membership is initialized only once per app start
            LazyInitializer.EnsureInitialized(ref InitializeSimpleMembershipSync._initializer,
                ref InitializeSimpleMembershipSync._isInitialized, 
                ref InitializeSimpleMembershipSync._initializerLock);
        }
    }
}
