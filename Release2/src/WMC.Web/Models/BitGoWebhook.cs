using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WMC.Web.Models
{
    public class BitGoWebhook
    {
        public string hash { get; set; }
        public string transfer { get; set; }
        public string coin { get; set; }
        public string type { get; set; }
        public string state { get; set; }
        public string wallet { get; set; }
    }
}