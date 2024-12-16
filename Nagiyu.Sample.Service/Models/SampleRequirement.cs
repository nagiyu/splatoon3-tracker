using Microsoft.AspNetCore.Authorization;

namespace Nagiyu.Sample.Service.Models
{
    public class SampleRequirement : IAuthorizationRequirement
    {
        public string SampleRole { get; }
    }
}
