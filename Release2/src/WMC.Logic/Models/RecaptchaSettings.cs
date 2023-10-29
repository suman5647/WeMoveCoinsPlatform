using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMC.Logic.Models
{
    public class RecaptchaAccessSettings
    {
        public int isEnabled { get; set; }

        public string Status { get; set; }

        public string RecaptchaPublicKey { get; set; }

        public string RecaptchaPrivateKey { get; set; }

    }
}
