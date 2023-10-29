using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMC.Logic.Models
{
    public class Input
    {
        public string id { get; set; }
        public string address { get; set; }
        public int value { get; set; }
        public string valueString { get; set; }
        public string wallet { get; set; }
        public int chain { get; set; }
        public int index { get; set; }
        public string redeemScript { get; set; }
        public bool isSegwit { get; set; }
    }

    public class Output
    {
        public string id { get; set; }
        public string address { get; set; }
        public int value { get; set; }
        public string valueString { get; set; }
        public string wallet { get; set; }
        public int chain { get; set; }
        public int index { get; set; }
        public string redeemScript { get; set; }
        public bool isSegwit { get; set; }
    }

    public class Entry
    {
        public string address { get; set; }
        public string wallet { get; set; }
        public string coinName { get; set; }
        public int inputs { get; set; }
        public int outputs { get; set; }
        public int value { get; set; }
        public string valueString { get; set; }
    }

    public class BitGoTransaction
    {
        public string id { get; set; }
        public string normalizedTxHash { get; set; }
        public DateTime date { get; set; }
        public string hex { get; set; }
        public string fromWallet { get; set; }
        public int blockHeight { get; set; }
        public int confirmations { get; set; }
        public int fee { get; set; }
        public string feeString { get; set; }
        public int size { get; set; }
        public List<string> inputIds { get; set; }
        public List<Input> inputs { get; set; }
        public List<Output> outputs { get; set; }
        public List<Entry> entries { get; set; }
    }
}
