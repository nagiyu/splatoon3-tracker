using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Nagiyu.Sample.Service.Handlers
{
    public static class SampleHandlerExtentions
    {
        public static void AddSampleHandler(this IServiceCollection options)
        {
            options.AddSingleton<IAuthorizationHandler, SampleHandler>();
        }
    }
}
