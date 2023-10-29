using MvcThrottle;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace WMC.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            var throttleFilter = new ThrottlingFilter
            {
                Policy = new ThrottlePolicy(perSecond: 2, perMinute: 10, perHour: 60 * 10, perDay: 600 * 10)
                {
                    //scope to IPs
                    IpThrottling = true,
                    //scope to clients
                    ClientThrottling = true,
                    //white list authenticated clients
                    //ClientWhitelist = new List<string> { "auth" },

                    //scope to requests path
                    EndpointThrottling = true,
                    EndpointType = EndpointThrottlingType.ControllerAndAction,
                    //EndpointRules = new Dictionary<string, RateLimits>
                    //{
                    //    { "home/", new RateLimits { PerHour = 90 } },
                    //    { "Home/about", new RateLimits { PerHour = 30 } }
                    //},

                    //scope to User-Agents
                    //UserAgentThrottling = true,
                    //UserAgentWhitelist = new List<string>
                    //{
                    //    "Googlebot",
                    //    "Mediapartners-Google",
                    //    "AdsBot-Google",
                    //    "Bingbot",
                    //    "YandexBot",
                    //    "DuckDuckBot"
                    //},
                    //UserAgentRules = new Dictionary<string, RateLimits>
                    //{
                    //    {"Facebot", new RateLimits { PerMinute = 1 }},
                    //    {"Sogou", new RateLimits { PerHour = 1 } }
                    //}
                },
            };

            filters.Add(throttleFilter);
        }
    }
}
