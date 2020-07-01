using System.Runtime.Serialization;

namespace SlackClient.Entities
{
    [DataContract]
    public class HistoryRoot
    {
        [DataMember(Name = "Messages")]
        public Message[] messages { get; set; }
    }
}