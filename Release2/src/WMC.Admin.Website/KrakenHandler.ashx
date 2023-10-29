<%@ WebHandler Language="C#" Class="KrakenHandler" %>

using System.Web;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

public class KrakenHandler : IHttpHandler
{
    public async void ProcessRequest(HttpContext context)
    {
        string query = context.Request.QueryString["query"];
        if (query != null && query.ToLower() == "balance")
        {
            context.Response.ContentType = "application/json";
            KrakenClient kragenClient = new KrakenClient();
            //var kragenBalance = await kragenClient.GetBalanceAsync();
            //if (kragenBalance != null && kragenBalance.Result != null && (kragenBalance.Errors == null || kragenBalance.Errors.Count == 0))
            //{
            //    context.Response.Write(JsonConvert.SerializeObject(kragenBalance));
            //    // context.Response.Write(string.Format("EUR: {0}, BTC: {1}", kragenBalance.Result.ZEUR, kragenBalance.Result.XXBT));
            //}
        }
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }


    public class KrakenClient : IDisposable
    {
        string _url;
        int _version;
        string _key;
        string _secret;
        //RateGate is was taken from http://www.jackleitch.net/2010/10/better-rate-limiting-with-dot-net/
        JackLeitch.RateGate.RateGate _rateGate;

        public KrakenClient()
        {
            _url = "https://api.kraken.com"; // ConfigurationManager.AppSettings["KrakenBaseAddress"];
            _version = 0; // int.Parse(ConfigurationManager.AppSettings["KrakenApiVersion"]);
            _key = "Lkt2A14swDyiAp5hVeJOTYPmGXbV1aqHfO6l9QlRXQrUqZVYBHMnWPH8"; // ConfigurationManager.AppSettings["KrakenKey"];
            _secret = "KXFuwATjexKVce5ePm2pGW/jufx6LSDxjaUPHG65jXhNdimL27rZ5PKGk1rhy6+J6eKLh58j5zvvoqbtIt1yZQ=="; // ConfigurationManager.AppSettings["KrakenSecret"];
            _rateGate = new JackLeitch.RateGate.RateGate(1, TimeSpan.FromSeconds(5));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_rateGate != null)
                    _rateGate.Dispose();
            }

            _rateGate = null;
        }

        ~KrakenClient()
        {
            Dispose(false);
        }

        #region Helper methods

        private byte[] sha256_hash(String value)
        {
            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;

                Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                return result;
            }
        }

        private byte[] getHash(byte[] keyByte, byte[] messageBytes)
        {
            using (var hmacsha512 = new HMACSHA512(keyByte))
            {

                Byte[] result = hmacsha512.ComputeHash(messageBytes);

                return result;

            }
        }


        #endregion


        private async Task<T> QueryPublicAsync<T>(string a_sMethod, string props = null)
        {
            string address = string.Format("{0}/{1}/public/{2}", _url, _version, a_sMethod);
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(address);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "POST";


            if (props != null)
            {

                using (var writer = new StreamWriter(webRequest.GetRequestStream()))
                {
                    writer.Write(props);
                }
            }

            //Make the request
            try
            {
                //Wait for RateGate
                _rateGate.WaitToProceed();

                using (WebResponse webResponse = await webRequest.GetResponseAsync())
                {
                    using (Stream str = webResponse.GetResponseStream())
                    {
                        using (StreamReader sr = new StreamReader(str))
                        {
                            var s = await sr.ReadToEndAsync();
                            return JsonConvert.DeserializeObject<T>(s);
                        }
                    }
                }
            }
            catch (WebException wex)
            {
                using (HttpWebResponse response = (HttpWebResponse)wex.Response)
                {
                    using (Stream str = response.GetResponseStream())
                    {
                        using (StreamReader sr = new StreamReader(str))
                        {
                            if (response.StatusCode != HttpStatusCode.InternalServerError)
                            {
                                throw;
                            }

                            return JsonConvert.DeserializeObject<T>(sr.ReadToEnd());
                        }
                    }
                }
            }
        }

        private async Task<T> QueryPrivateAsync<T>(string a_sMethod, string props = null)
        {
            // generate a 64 bit nonce using a timestamp at tick resolution
            Int64 nonce = DateTime.Now.Ticks;
            props = "nonce=" + nonce + props;


            string path = string.Format("/{0}/private/{1}", _version, a_sMethod);
            string address = _url + path;
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(address);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "POST";
            webRequest.Headers.Add("API-Key", _key);


            byte[] base64DecodedSecred = Convert.FromBase64String(_secret);

            var np = nonce + Convert.ToChar(0) + props;

            var pathBytes = Encoding.UTF8.GetBytes(path);
            var hash256Bytes = sha256_hash(np);
            var z = new byte[pathBytes.Count() + hash256Bytes.Count()];
            pathBytes.CopyTo(z, 0);
            hash256Bytes.CopyTo(z, pathBytes.Count());

            var signature = getHash(base64DecodedSecred, z);

            webRequest.Headers.Add("API-Sign", Convert.ToBase64String(signature));

            if (props != null)
            {

                using (var writer = new StreamWriter(webRequest.GetRequestStream()))
                {
                    writer.Write(props);
                }
            }

            //Make the request
            try
            {
                //Wait for RateGate
                _rateGate.WaitToProceed();

                using (WebResponse webResponse = webRequest.GetResponse())
                {
                    using (Stream str = webResponse.GetResponseStream())
                    {
                        using (StreamReader sr = new StreamReader(str))
                        {
                            var s = await sr.ReadToEndAsync();
                            return JsonConvert.DeserializeObject<T>(s);
                        }
                    }
                }
            }
            catch (WebException wex)
            {
                using (HttpWebResponse response = (HttpWebResponse)wex.Response)
                {
                    using (Stream str = response.GetResponseStream())
                    {
                        using (StreamReader sr = new StreamReader(str))
                        {
                            if (response.StatusCode != HttpStatusCode.InternalServerError)
                            {
                                throw;
                            }

                            var s = sr.ReadToEnd();
                            return JsonConvert.DeserializeObject<T>(s);
                        }
                    }
                }

            }
        }

        public async Task<KrakenBalance> GetBalanceAsync()
        {
            return await QueryPrivateAsync<KrakenBalance>("Balance");
        }
    }

    public class KrakenBalance
    {
        // {\"error\":[],\"result\":{\"ZEUR\":\"64863.0519\",\"XXBT\":\"1.5000024263\",\"XETH\":\"0.0000000000\",\"BCH\":\"0.0000089400\"}}
        // "{\"error\":[\"EAPI:Invalid nonce\"]}"
        [JsonProperty("result")]
        public BalanceResult Result { get; set; }
        [JsonProperty("error")]
        public KrakenErrors Errors { get; set; }
    }

    public class KrakenErrors : List<string>
    {
    }

    // public class KrakenError
    // {
    //     public string EAPI { get; set; }
    // }

    public class BalanceResult
    {
        public double ZEUR { get; set; }
        public double XXBT { get; set; }
        public double XETH { get; set; }
        public double BCH { get; set; }
    }
}