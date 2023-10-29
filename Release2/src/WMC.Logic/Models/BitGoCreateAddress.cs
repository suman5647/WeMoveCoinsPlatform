using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMC.Logic.Models
{
    public class BitGoCreateAddress
    {
        public string id { get; set; }
        public string address { get; set; }
        public int chain { get; set; }
        public int index { get; set; }
        public string coin { get; set; }
        public string wallet { get; set; }
        public CoinSpecific2 coinSpecific { get; set; }
    }
}
