using System.Runtime.Serialization;

namespace SlackClient.Entities
{
    [DataContract]
    public class Message
    {
        [DataMember(Name = "Type")]
        public string type { get; set; }

        [DataMember(Name = "User")]
        public string user { get; set; }

        [DataMember(Name = "Text")]
        public string text { get; set; }

        [DataMember(Name = "Timestamp")]
        public string ts { get; set; }
    }
}