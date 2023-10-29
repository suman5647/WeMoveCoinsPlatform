using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMC.Logic.Models
{
    public class KrakenAccount
    {
        public string KrakenBaseAddress { get; set; }

        public string KrakenApiVersion { get; set; }

        public string KrakenKey { get; set; }

        public string KrakenSecret { get; set; }
    }
}
