using RestSharp;
using System;
using System.Collections.Generic;

namespace BitGoSharp
{
    public class Wallet
    {
        public class WalletObject
        {
            public string _id { get; set; }
            public string id { get; set; }
            public string label { get; set; }
            public bool isActive { get; set; }
            public string type { get; set; }
            public Freeze freeze { get; set; }
            public int adminCount { get; set; }
            public bool disableTransactionNotifications { get; set; }
            public Private @private { get; set; }
            public bool canSendInstant { get; set; }
            public string permissions { get; set; }
            public Admin admin { get; set; }
            public List<object> tags { get; set; }
            public int approvalsRequired { get; set; }
            public bool spendingAccount { get; set; }
            public List<object> pendingApprovals { get; set; }
            public long balance { get; set; }
            public long instantBalance { get; set; }
            public long spendableConfirmedBalance { get; set; }
            public long confirmedBalance { get; set; }
            public long spendableBalance { get; set; }
            public long sent { get; set; }
            public long received { get; set; }
            public long unconfirmedSends { get; set; }
            public long unconfirmedReceives { get; set; }
        }
        public class Freeze
        {
        }
        public class Params
        {
            public string pubKey { get; set; }
            public string chainCode { get; set; }
            public int depth { get; set; }
            public int index { get; set; }
            public long parentFingerprint { get; set; }
        }
        public class Keychain
        {
            public string xpub { get; set; }
            public string path { get; set; }
            public Params @params { get; set; }
        }
        public class Private
        {
            public List<Keychain> keychains { get; set; }
        }
        public class User
        {
            public string user { get; set; }
            public string permissions { get; set; }
        }
        public class Admin
        {
            public List<User> users { get; set; }
        }

        string url;
        BitGoClient bitGoClientInstance;
        TransactionBuilder transactionBuilder;
        public WalletObject walletInstance { get; set; }
        RestClient restClient;
        public Wallet(RestClient client, BitGoClient bitGoClient, string walletId)
        {
            restClient = client;
            url = client.BaseUrl.OriginalString;
            client.BaseUrl = new Uri(url + "/wallet/" + walletId);
            var response = client.Get(new RestRequest());
            bitGoClientInstance = bitGoClient;
            try
            {
                walletInstance = SimpleJson.DeserializeObject<WalletObject>(response.Content);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception occured while deserializing wallet initialization response. Initialization response for wallet " + walletId + ": " + response.Content, ex);
            }
        }

        public IRestResponse SendCoins(string address, double amount, string walletPassphrase, int minConfirms)
        {
            var recipients = new Dictionary<string, double>();
            recipients.Add(address, amount);
            var transaction = CreateAndSignTransaction(new { address, amount, recipients, walletPassphrase, minConfirms });
            return SendTransaction(transaction.transactionHex, (long)transaction.estimatedSize, (long)transaction.feeRate, (long)transaction.fee);
        }

        public dynamic CreateAndSignTransaction(string address, double amount, string walletPassphrase, int minConfirms)
        {
            var recipients = new Dictionary<string, double>();
            recipients.Add(address, amount);
            transactionBuilder = new TransactionBuilder(this, bitGoClientInstance);
            var keychain = GetAndPrepareSigningKeychain(walletPassphrase);
            var transaction = CreateTransaction(new { address, amount, recipients, walletPassphrase, minConfirms });
            var transactionHex = SignTransaction(keychain, transaction);
            return new { transactionHex = transactionHex, estimatedSize = transaction.estimatedSize, feeRate = transaction.feeRate, fee = transaction.fee };
        }

        public WalletTransaction GetTransaction(string id)
        {
            restClient.BaseUrl = new Uri(restClient.BaseUrl + "/tx/" + id);
            var response = restClient.Get(new RestRequest());
            var transaction = new WalletTransaction();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                transaction = SimpleJson.DeserializeObject<WalletTransaction>(response.Content);
            return transaction;
        }

        public dynamic CreateAndSignTransaction(dynamic param)
        {
            //param.recipients
            transactionBuilder = new TransactionBuilder(this, bitGoClientInstance);
            var keychain = GetAndPrepareSigningKeychain(param.walletPassphrase);
            var transaction = CreateTransaction(new { param.address, param.amount, param.recipients, param.walletPassphrase, param.minConfirms });
            var transactionHex = SignTransaction(keychain, transaction);
            return new { transactionHex = transactionHex, estimatedSize = transaction.estimatedSize, feeRate = transaction.feeRate, fee = transaction.fee };
        }

        WalletKeychain.WalletKeychainObject GetAndPrepareSigningKeychain(string walletPassphrase)
        {
            restClient.BaseUrl = new Uri(url);
            var keychain = new WalletKeychain(restClient, walletInstance.@private.keychains[0]);
            keychain.Instance.xprv = new SJCLDecryptor(keychain.Instance.encryptedXprv, walletPassphrase).Plaintext;
            return keychain.Instance;
        }

        dynamic CreateTransaction(dynamic param)
        {
            return transactionBuilder.CreateTransaction(new { param.address, param.amount, param.recipients, param.walletPassphrase, param.minConfirms, validate = true });
        }

        dynamic SignTransaction(dynamic keychain, dynamic transaction)
        {
            return transactionBuilder.SignTransaction(new
            {
                transaction.transactionHex,
                transaction.unspents,
                transaction.fee,
                transaction.changeAddresses,
                transaction.walletId,
                transaction.walletKeychains,
                transaction.feeRate,
                transaction.feeSingleKeyWIF,
                transaction.instant,
                transaction.bitgoFee,
                transaction.estimatedSize,
                transaction.travelInfos,
                keychain
            });
        }

        public string Addresses()
        {
            restClient.BaseUrl = new Uri(url + "/wallet/" + walletInstance.id + "/addresses/" + 1);
            var addr = restClient.Get(new RestRequest()).Content;
            // Validate Address
            return addr;
        }

        public string CreateAddress(dynamic param)
        {
            var chain = param.chain ?? 0;
            restClient.BaseUrl = new Uri(url + "/wallet/" + walletInstance.id + "/address/" + chain);
            var request = new RestRequest();
            request.AddJsonBody(new
            {
                chain = param.chain,
                validate = param.validate,
            });
            var response = restClient.Post(request);
            dynamic address = SimpleJson.DeserializeObject(response.Content);
            return address.address;
        }

        //public string AddWebhook(dynamic param)
        //{
        //    var chain = param.chain ?? 0;
        //    restClient.BaseUrl = new Uri(url + "/wallet/" + walletInstance.id + "/address/" + chain);
        //    var request = new RestRequest();
        //    request.AddJsonBody(new
        //    {
        //        chain = param.chain,
        //        validate = param.validate,
        //    });
        //    var response = restClient.Post(request);
        //    dynamic address = SimpleJson.DeserializeObject(response.Content);
        //    return address.address;
        //}

        public string CreateAddress(int chain, bool validate)
        {
            restClient.BaseUrl = new Uri(url + "/wallet/" + walletInstance.id + "/address/" + chain);
            var request = new RestRequest();
            request.AddJsonBody(new { chain = chain, validate = validate });
            var response = restClient.Post(request);
            dynamic address = SimpleJson.DeserializeObject(response.Content);
            return address.address;
        }

        IRestResponse SendTransaction(string tx, long estimatedSize, long feeRate, long fee)
        {
            restClient.BaseUrl = new Uri(url + "/tx/send");
            var request = new RestRequest();
            request.AddJsonBody(new
            {
                tx = tx,
                estimatedSize = estimatedSize,
                feeRate = feeRate,
                fee = fee
            });
            return restClient.Post(request);
        }

        public IRestResponse GetBitGoFee(decimal amount, string instant)
        {
            restClient.BaseUrl = new Uri(url + "/wallet/" + this.walletInstance.id + "/billing/fee");
            var request = new RestRequest();
            request.AddParameter("amount", amount);
            request.AddParameter("instant", instant);
            return restClient.Get(request);
        }

        public IRestResponse UnspentsPaged(string instant, int minSize, decimal target, string targetWalletUnspents)
        {
            restClient.BaseUrl = new Uri(url + "/wallet/" + this.walletInstance.id + "/unspents");
            var request = new RestRequest();
            request.AddParameter("minSize", minSize);
            request.AddParameter("instant", instant);
            request.AddParameter("target", target);
            if (targetWalletUnspents != null)
                request.AddParameter("targetWalletUnspents", targetWalletUnspents);
            return restClient.Get(request);
        }
    }
}