using Microsoft.AspNetCore.Authorization;
using Nagiyu.Sample.Service.Models;

namespace Nagiyu.Sample.Service.Policies
{
    public static class SamplePolicy
    {
        public static void AddSamplePolicy(this AuthorizationOptions options)
        {
            options.AddPolicy("SamplePolicy", policy =>
            {
                policy.Requirements.Add(new SampleRequirement());
            });
        }
    }
}
