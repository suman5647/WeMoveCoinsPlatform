using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WMC.Logic;

namespace WMC.Helpers
{
    public static class ResourceExtensions
    {
        private static IResourceService ResourceService = StringResourceService.GetDefault();

        public static IHtmlString HtmlStringResource(this HtmlHelper htmlhelper, string resourceClass, string resourceKey, Dictionary<string, string> replaceValues = null)
        {
            var resultStr = Resource(htmlhelper.ViewContext.HttpContext, resourceClass, resourceKey, replaceValues);
            return htmlhelper.Raw(string.IsNullOrEmpty(resultStr) ? "" : resultStr);
        }
        public static IHtmlString HtmlStringResourceFormated(this HtmlHelper htmlhelper, string resourceClass, string resourceKey, params object[] args)

        {
            var resultStr = Resource(htmlhelper.ViewContext.HttpContext, resourceClass, resourceKey, args);
            return htmlhelper.Raw(string.IsNullOrEmpty(resultStr) ? "" : resultStr);
        }

        public static string Resource(this HtmlHelper htmlhelper, string resourceClass, string resourceKey, Dictionary<string, string> replaceValues = null)
        {
            var resultStr = Resource(htmlhelper.ViewContext.HttpContext, resourceClass, resourceKey, replaceValues);
            return resultStr;
        }

        public static string StringResource(this HtmlHelper htmlhelper, string resourceClass, string resourceKey, Dictionary<string, string> replaceValues = null)
        {
            var resultStr = Resource(htmlhelper.ViewContext.HttpContext, resourceClass, resourceKey, replaceValues);
            return resultStr;
        }

        public static string Resource(HttpContextBase httpContext, string resourceClass, string resourceKey, Dictionary<string, string> replaceValues = null)
        {
            if (replaceValues == null) replaceValues = new Dictionary<string, string>();
            var tvalues = ResolveLanguageAndHostName(httpContext, resourceClass, resourceKey);
            var resultStr = Resource(resourceKey, tvalues.Item1, tvalues.Item2, replaceValues);
            return resultStr;
        }
        
        public static string Resource(HttpContextBase httpContext, string resourceClass, string resourceKey, params object[] args)
        {
            var tvalues = ResolveLanguageAndHostName(httpContext, resourceClass, resourceKey);
            var resultStr = FormatedResource(resourceKey, tvalues.Item1, tvalues.Item2, args);
            return resultStr;
        }

        private static Tuple<string, string> ResolveLanguageAndHostName(HttpContextBase httpContext, string resourceClass, string resourceKey)
        {
            var language = "";
            var userLanguages = httpContext.Request?.UserLanguages;
            if (userLanguages != null)
            {
                CultureInfo culture = StringResourceService.ResolveCulture(userLanguages[0]);
                if (culture.Name != null && culture.Name.Count() > 0)
                {
                    language = culture.Name.Split('-')[0];
                }
            }
            var hostname = httpContext.Request.Url.Host;
            if (!hostname.Contains("localhost"))
            {
                var _hosts = hostname.Split('.');
                if (_hosts.Length > 2)
                    hostname = _hosts[_hosts.Length - 2];
            }

            return new Tuple<string, string>(language, hostname);
        }

        private static string Resource(string resourceKey, string language = "en", string hostname = "localhost", Dictionary<string, string> replaceValues = null)
        {
            if (replaceValues == null) replaceValues = new Dictionary<string, string>();
            if (!hostname.Contains("localhost"))
            {
                var _hosts = hostname.Split('.');
                if (_hosts.Length > 2)
                    hostname = _hosts[_hosts.Length - 2];
            }

            if (language == "en" || hostname.Contains("localhost"))
                language = "";
            //else if (!string.IsNullOrEmpty(language)) //if language is not blank, append '.'; else leave blank language as is.
            //    language = "." + language;

            var resourceValue = ResourceService.Get(resourceKey, language, replaceValues);
            if (resourceValue != null)
            {
                return resourceValue.Value;
            }
            else
            {
                AuditLog.log(string.Format("Error in Get Resource() not found: class:{0},key:{1},lang:{2},urlHost:{3},host:{4}", "CLASS", resourceKey, language, "_NA_", hostname), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Warn);
                return resourceKey;
            }
        }

        private static string FormatedResource(string resourceKey, string language = "en", string hostname = "localhost", params object[] args)
        {
            if (!hostname.Contains("localhost"))
            {
                var _hosts = hostname.Split('.');
                if (_hosts.Length > 2)
                    hostname = _hosts[_hosts.Length - 2];
            }

            if (language == "en" || hostname.Contains("localhost"))
                language = "";
            //else if (!string.IsNullOrEmpty(language)) //if language is not blank, append '.'; else leave blank language as is.
            //    language = "." + language;

            var resourceValue = ResourceService.GetFormated(resourceKey, language, args);
            if (resourceValue != null)
            {
                return resourceValue.Value;
            }
            else
            {
                AuditLog.log(string.Format("Error in Get Resource() not found: class:{0},key:{1},lang:{2},urlHost:{3},host:{4}", "CLASS", resourceKey, language, "_NA_", hostname), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Warn);
                return resourceKey;
            }
        }
    }

    public interface IResourceService
    {
        ResourceValue Get(string key, string language);
        // IEnumerable<ResourceValue> Get(string key);
        ResourceValue Get(string key, CultureInfo culture);
        ResourceValue GetFormated(string key, string language, params object[] args);
        ResourceValue Get(string key, string language, Dictionary<string, string> transform);
        ResourceValue Get(string key, CultureInfo culture, Dictionary<string, string> transform);
    }

    public class StringResourceService : IResourceService
    {
        private IResourceManager resourceManager;

        public StringResourceService(IResourceManager resourceManager)
        {
            this.resourceManager = resourceManager;
        }

        public static IResourceService GetDefault()
        {
            return new StringResourceService(new StringResourceManager(new MemCaching()));
        }

        public ResourceValue Get(string key, string language)
        {
            return resourceManager.Get(key, language);
        }

        public ResourceValue Get(string key, CultureInfo culture)
        {
            return Get(key, culture.Name.Contains("-") ? culture.Name.Split('-')[1] : culture.Name);
        }

        public ResourceValue GetFormated(string key, string language, params object[] args)
        {
            CultureInfo culture = ResolveCulture(language);
            var resourceVal = Get(key, language);
            if (resourceVal == null) return null;            
            //resourceVal.Value = (args.Length > 0) ? string.Format(culture, "{0} {1} {2:D} ", resourceVal.Value, args[0], args[1]) : string.Format(culture, resourceVal.Value);
            resourceVal.Value = string.Format(culture, resourceVal.Value, args);
            return resourceVal;
        }

        public ResourceValue Get(string key, string language, Dictionary<string, string> transform)
        {
            var resourceVal = Get(key, language);
            if (resourceVal == null) return null;
            resourceVal.Value = Transform(resourceVal.Value, transform);
            return resourceVal;
        }

        public ResourceValue Get(string key, CultureInfo culture, Dictionary<string, string> transform)
        {
            var resourceVal = Get(key, culture);
            if (resourceVal == null) return null;
            resourceVal.Value = Transform(resourceVal.Value, transform);
            return resourceVal;
        }

        private static string Transform(string val, Dictionary<string, string> transform)
        {
            if (transform != null && transform.Any())
            {
                foreach (var transformPair in transform)
                {
                    val = val.Replace(transformPair.Key, transformPair.Value);
                }
            }

            return val;
        }

        /// <summary>
        /// TODOL simplify
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        public static CultureInfo ResolveCulture(string lang)
        {
            CultureInfo cultureInfo;
            switch (lang)
            {
                case "da":
                    cultureInfo = new CultureInfo("da-DK");
                    break;
                case "de":
                    cultureInfo = new CultureInfo("de-DE");
                    break;
                case "es":
                    cultureInfo = new CultureInfo("es-ES");
                    break;
                case "fr":
                    cultureInfo = new CultureInfo("fr-FR");
                    break;
                default:
                    cultureInfo = new CultureInfo("en-US");
                    break;
            }

            return cultureInfo;
        }
    }
}