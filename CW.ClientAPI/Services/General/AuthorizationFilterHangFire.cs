using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire.Dashboard;

namespace CW.ClientAPI.Services.General
{
    public class AuthorizationFilterHangFire : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

             //Allow all authenticated users to see the dashboard
            //  return httpContext.User.Identity.IsAuthenticated;
            return true;
        }

    }
}
