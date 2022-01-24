using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.Logger
{
    public class Body
    {
        public string type { get; set; }
        public List<Item> items { get; set; }
    }

    public class Content
    {
        [JsonProperty("$schema")]
        public string Schema { get; set; }
        public string type { get; set; }
        public string version { get; set; }
        public List<Body> body { get; set; }
    }

    public class Attachment
    {
        public string contentType { get; set; }
        public Content content { get; set; }
    }

    public class Message
    {
        public string type { get; set; }
        public List<Attachment> attachments { get; set; }
    }

    public class Item
    {
        public string type { get; set; }
        public string url { get; set; }
        public string size { get; set; }
        public string text { get; set; }
        public string weight { get; set; }
        public bool? wrap { get; set; }
        public string spacing { get; set; }
        public bool? isSubtle { get; set; }
        public string height { get; set; }
        public string width { get; set; }
        public List<Column> columns { get; set; }
    }

    public class Column
    {
        public string type { get; set; }
        public string width { get; set; }
        public List<Item> items { get; set; }
    }
}
