using Newtonsoft.Json;
using System.Collections.Generic;

namespace WMC.Logic.Models.Settings
{
    public class BitGoMinersFeeSettings
    {
        public string DefaultWalletId { get; set; }
        public double DefaultAmount { get; set; }
        public double MinersFee { get; set; }
    }
    public class BitGoCurrencySettings
    {
        public string DefaultWalletId { get; set; }
        public decimal DefaultAmount { get; set; }
        public string TestCurrency { get; set; }
        public string KrakenCode { get; set; }
        public string KrakenEurPairCode { get; set; }
        public MinersFee MinersFee { get; set; }
        public string PassPhrase { get; set; }
        public long TxUnit { get; set; }
        public List<SellMessageLanguageResource> SellMessageLangRes { get; set; }
    }

    public class MinersFee
    {
        [JsonProperty("size")]
        public int Size { get; set; }
        [JsonProperty("feeRate")]
        public int FeeRate { get; set; }
        [JsonProperty("fee")]
        public decimal Fee { get; set; }
        [JsonProperty("payGoFee")]
        public int PayGoFee { get; set; }
        [JsonProperty("payGoFeeString")]
        public string PayGoFeeString { get; set; }
    }

    public class SellMessageLanguageResource
    {
        public decimal MinAmountInEUR { get; set; }
        public decimal MaxAmountInEUR { get; set; }
        public string LanguageResource { get; set; }
    }

}
