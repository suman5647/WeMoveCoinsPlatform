using System;
using System.Collections.Generic;

namespace BitGoSharp
{
    public class Entry
    {
        public string account { get; set; }
        public int value { get; set; }
    }

    public class Output
    {
        public string account { get; set; }
        public bool isMine { get; set; }
        public int chain { get; set; }
        public int chainIndex { get; set; }
        public int value { get; set; }
        public int vout { get; set; }
    }

    public class WalletTransaction
    {
        public string blockhash { get; set; }
        public int confirmations { get; set; }
        public DateTime date { get; set; }
        public List<Entry> entries { get; set; }
        public int fee { get; set; }
        public int height { get; set; }
        public string hex { get; set; }
        public string id { get; set; }
        public List<Output> outputs { get; set; }
        public bool pending { get; set; }
        public bool instant { get; set; }
        public string instantId { get; set; }
        public string sequenceId { get; set; }
        public string comment { get; set; }
        public List<string> replayProtection { get; set; }
    }
}
