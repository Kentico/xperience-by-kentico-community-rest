using System.Net.Http.Headers;
using System.Text;

using Kentico.Xperience.Admin.Base.Authentication.Internal;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Xperience.Community.Rest.Attributes
{
    /// <summary>
    /// Performs Basic authentication and returns a <see cref="UnauthorizedResult"/> if header is missing or invalid.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RestAuthenticationAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var headerValues))
            {
                context.Result = new UnauthorizedResult();

                return;
            }

            string? authHeader = headerValues.FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader))
            {
                context.Result = new UnauthorizedResult();

                return;
            }

            var authHeaderVal = AuthenticationHeaderValue.Parse(authHeader);
            bool signedIn = await AuthenticateUser(authHeaderVal.Parameter, context);
            if (!signedIn)
            {
                context.Result = new UnauthorizedResult();
            }
        }


        private static Task<bool> AuthenticateUser(string? credentials, AuthorizationFilterContext context)
        {
            if (string.IsNullOrEmpty(credentials))
            {
                return Task.FromResult(false);
            }

            try
            {
                var encoding = Encoding.GetEncoding("iso-8859-1");
                credentials = encoding.GetString(Convert.FromBase64String(credentials));

                int separator = credentials.IndexOf(':');
                string name = credentials[..separator];
                string password = credentials[(separator + 1)..];

                return CheckPassword(name, password, context);
            }
            catch (Exception)
            {
                return Task.FromResult(false);
            }
        }


        private static async Task<bool> CheckPassword(string username, string password, AuthorizationFilterContext context)
        {
            var signInManager = context.HttpContext.RequestServices.GetService<AdminSignInManager>() ??
                throw new InvalidOperationException($"Unabled to resolve service '{nameof(AdminSignInManager)}.'");
            var result = await signInManager.PasswordSignInAsync(username, password, false, false);

            return result.Succeeded;
        }
    }
}
