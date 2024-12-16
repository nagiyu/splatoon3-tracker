using Microsoft.AspNetCore.Authorization;
using Nagiyu.Common.Auth.Service.Services;
using Nagiyu.Sample.Service.Enums;
using Nagiyu.Sample.Service.Models;
using System.Threading.Tasks;

namespace Nagiyu.Sample.Service.Handlers
{
    public class SampleHandler : AuthorizationHandler<SampleRequirement>
    {
        private readonly AuthService authService;

        public SampleHandler(AuthService authService)
        {
            this.authService = authService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, SampleRequirement requirement)
        {
            var user = await authService.GetUser<SampleAuth>();

            if (user == null)
            {
                context.Fail();
                return;
            }

            if (user.SampleRole == SampleRole.Admin)
            {
                context.Succeed(requirement);
            }
        }
    }
}
