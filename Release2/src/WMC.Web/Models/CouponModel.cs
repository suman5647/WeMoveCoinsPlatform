using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WMC.Web.Models
{
    public class CouponModel
    {
        public bool Validity { get; set; }
        public decimal Discount { get; set; }
        public string ErrorMessage { get; set; }
    }
}