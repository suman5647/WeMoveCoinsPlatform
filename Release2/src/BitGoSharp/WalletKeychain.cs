using RestSharp;
using RestSharp.Extensions.MonoHttp;
using System;

namespace BitGoSharp
{
    internal class WalletKeychain
    {
        string url;
        RestClient restClient;
        public WalletKeychainObject Instance { get; set; }
        public WalletKeychain(RestClient client, Wallet.Keychain keychain)
        {
            restClient = client;
            url = client.BaseUrl.OriginalString;
            client.BaseUrl = new Uri(url + "/keychain/" + HttpUtility.UrlEncode(keychain.xpub));
            var response = client.Post(new RestRequest());
            Instance = SimpleJson.DeserializeObject<WalletKeychainObject>(response.Content);
            Instance.walletSubPath = keychain.path;
        }

        internal class WalletKeychainObject
        {
            public string xpub { get; set; }
            public string ethAddress { get; set; }
            public string encryptedXprv { get; set; }
            public string path { get; set; }
            public string walletSubPath { get; set; }
            public string xprv { get; set; }
        }
    }
}