using Hangfire.Dashboard;

namespace LaundrySystem.Api.BackgroundServices
{
    /// <summary>
    /// Authorization filter for Hangfire dashboard. Only allows admin users.
    /// </summary>
    public class HangfireDashboardAuthFilter : IDashboardAuthorizationFilter
    {
        /// <summary>
        /// Authorizes access to the Hangfire dashboard.
        /// </summary>
        /// <param name="context">The dashboard context.</param>
        /// <returns>True if the user is authorized.</returns>
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            var user = httpContext.User;

            return user.Identity?.IsAuthenticated == true
                && (user.IsInRole("AccountAdmin")
                    || user.IsInRole("BuildingAdmin")
                    || user.IsInRole("SuperAdmin"));
        }
    }
}
