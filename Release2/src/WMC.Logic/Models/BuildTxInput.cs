using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMC.Logic.Models
{
    class BuildTxInput
    {
        public List<Recipients> recipients { get; set; }

        public BuildTxInput(List<Recipients> recipients)
        {
            this.recipients = recipients;
        }
    }

    class Recipients
    {
        public string address { get; set; }

        public long amount { get; set; }

        public Recipients(string address, long amount)
        {
            this.address = address;
            this.amount = amount;
        }
    }
}
