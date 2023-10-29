using System;
using Newtonsoft.Json;
using RestSharp;
using WMC.Data;
using WMC.Utilities;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using WMC.Logic;

namespace WMC.Web.Utilities
{
    public class IPInfoService
    {
        public static string GetIPInfo(string ipAddress, int? orderId = null)
        {
            string countryCode = default(string);
            try
            {
                IGeoLocationProvider geolocationProvider = new IPAPI(ipAddress);
                countryCode = new Retrier<string>().Try(() => geolocationProvider.GetIPInfo(), 3);
                if (default(string) == countryCode)
                {
                    AuditLog.log(string.Format("Response failed from GeoLocationProvider - http://ip-api.com/ for IP {0}", ipAddress), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error, orderId);
                    geolocationProvider = new IPInfo(ipAddress);
                    countryCode = new Retrier<string>().Try(() => geolocationProvider.GetIPInfo(), 3);
                }
                if (default(string) == countryCode)
                {
                    AuditLog.log(string.Format("Response failed from GeoLocationProvider - http://ipinfo.io/ for IP {0}", ipAddress), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error, orderId);
                    throw new Exception("Response failed from GeoLocationProvider");
                }
            }
            catch (System.Exception)
            {
                countryCode = "DK";
            }
            return countryCode;
        }

        public static string GetIPInfoJSON(string ipAddress)
        {
            try
            {
                var ipinfourl = "http://ipinfo.io/" + ipAddress + "/json";
                var client = new RestClient(ipinfourl);
                var request = new RestRequest(Method.GET);
                return client.Execute(request).Content;
            }
            catch (System.Exception)
            {
                return "DK";
            }
        }
    }

    public class IPInfoDetail
    {
        public string ip { get; set; }
        public string hostname { get; set; }
        public string city { get; set; }
        public string region { get; set; }
        public string country { get; set; }
        public string loc { get; set; }
        public string org { get; set; }
    }

    public interface IGeoLocationProvider
    {
        string Uri { get; set; }
        string GetIPInfo();
    }

    public class IPAPI : IGeoLocationProvider
    {
        private string uri;
        public string Uri { get { return uri; } set { uri = "http://ip-api.com/json/" + value; } }

        public IPAPI()
        {
        }

        public IPAPI(string ip)
        {
            this.Uri = ip;
        }

        public string GetIPInfo()
        {
            try
            {
                var client = new RestClient(Uri);
                var request = new RestRequest(Method.GET);
                var response = client.Execute(request);
                return JsonConvert.DeserializeObject<dynamic>(response.Content).countryCode.ToString();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }

    public class IPInfo : IGeoLocationProvider
    {
        private string uri;
        public string Uri { get { return uri; } set { uri = "http://ipinfo.io/" + value + "/json"; } }

        public IPInfo()
        {
        }

        public IPInfo(string ip)
        {
            this.Uri = ip;
        }

        public string GetIPInfo()
        {
            try
            {
                var client = new RestClient(Uri);
                var request = new RestRequest(Method.GET);
                var response = client.Execute(request);
                return JsonConvert.DeserializeObject<dynamic>(response.Content).country.ToString();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}