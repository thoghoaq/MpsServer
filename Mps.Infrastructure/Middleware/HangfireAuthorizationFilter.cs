using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace Mps.Infrastructure.Middleware
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            return true;
        }
    }
}
