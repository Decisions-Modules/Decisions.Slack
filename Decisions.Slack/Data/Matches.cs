using System.Runtime.Serialization;

namespace SlackClient.Entities
{
    [DataContract]
    public class Matches
    {
        [DataMember(Name = "Channel")]
        public Channel channel { get; set; }

        [DataMember(Name = "Type")]
        public string type { get; set; }

        [DataMember(Name = "User")]
        public string user { get; set; }

        [DataMember(Name = "Text")]
        public string text { get; set; }

        [DataMember(Name = "Timestamp")]
        public string ts { get; set; }

        [DataMember(Name = "Permalink")]
        public string permalink { get; set; }

        [DataMember(Name = "Team")]
        public string team { get; set; }

        [DataMember(Name = "Iid")]
        public string iid { get; set; }
    }
}