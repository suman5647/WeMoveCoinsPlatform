using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMC.Logic.Models
{
    public class User
    {
        public string user { get; set; }
        public List<string> permissions { get; set; }
    }

    public class KeySignatures
    {
        public string bitgoPub { get; set; }
        public string backupPub { get; set; }
    }

    public class Freeze
    {
    }

    public class CoinSpecific
    {
    }

    public class Policy
    {
        public string id { get; set; }
        public string label { get; set; }
        public int version { get; set; }
        public DateTime date { get; set; }
        public List<object> rules { get; set; }
    }

    public class Admin
    {
        public Policy policy { get; set; }
    }

    public class CoinSpecific2
    {
        public string redeemScript { get; set; }
    }

    public class ReceiveAddress
    {
        public string id { get; set; }
        public string address { get; set; }
        public int chain { get; set; }
        public int index { get; set; }
        public string coin { get; set; }
        public string wallet { get; set; }
        public CoinSpecific2 coinSpecific { get; set; }
    }

    public class BitGoWallet
    {
        public string id { get; set; }
        public List<User> users { get; set; }
        public string coin { get; set; }
        public string label { get; set; }
        public int m { get; set; }
        public int n { get; set; }
        public List<string> keys { get; set; }
        public KeySignatures keySignatures { get; set; }
        public List<string> tags { get; set; }
        public bool disableTransactionNotifications { get; set; }
        public Freeze freeze { get; set; }
        public bool triggeredCircuitBreaker { get; set; }
        public bool deleted { get; set; }
        public int approvalsRequired { get; set; }
        public bool isCold { get; set; }
        public CoinSpecific coinSpecific { get; set; }
        public Admin admin { get; set; }
        public List<object> clientFlags { get; set; }
        public bool allowBackupKeySigning { get; set; }
        public long balance { get; set; }
        public long confirmedBalance { get; set; }
        public long spendableBalance { get; set; }
        public string balanceString { get; set; }
        public string confirmedBalanceString { get; set; }
        public string spendableBalanceString { get; set; }
        public ReceiveAddress receiveAddress { get; set; }
        public List<object> pendingApprovals { get; set; }
    }
}
