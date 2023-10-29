using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMC.Logic.Models
{
    class SendCoinInput
    {
        public string address { get; set; }
        public long amount { get; set; }
        public string walletPassphrase { get; set; }
        public int minConfirms { get; set; }

        public SendCoinInput(string address, long amount, string walletPassphrase, int minConfirms)
        {
            this.address = address;
            this.amount = amount;
            this.minConfirms = minConfirms;
            this.walletPassphrase = walletPassphrase;
        }
    }
}
