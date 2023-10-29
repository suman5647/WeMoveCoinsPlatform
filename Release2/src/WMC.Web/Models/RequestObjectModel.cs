using System.Collections.Generic;

namespace WMC.Web.Models
{
    public class Header
    {
        public List<KeyValuePair<string, string>> RequestHeader { get; set; }

        public Header()
        {
            RequestHeader = new List<KeyValuePair<string, string>>();
        }
    }
    public class RequestObjectModel
    {
        public Header header { get; set; }
        public string body { get; set; }
    }
}