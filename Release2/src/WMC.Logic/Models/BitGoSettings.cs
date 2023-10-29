using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMC.Logic.Models
{
    public class BitGoAccessSettings
    {
        public string Environment { get; set; }

        public string AccessCode { get; set; }

        public string TOTPKey { get; set; }

        public DateTime? SessionExpiry { get; set; }

        public string Url { get; set; }
        
    }
}
