using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMC.Logic.Models
{
    public class TwilioDetails
    {
        public string From { get; set; }

        public string FromNumber { get; set; }

        public string AccountSid { get; set; }

        public string AuthToken { get; set; }

        public string Message { get; set; }
    }
}
