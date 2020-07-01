using System.Runtime.Serialization;

namespace SlackClient.Entities
{
    [DataContract]
    public class ChannelsList
    {
        [DataMember(Name = "Channels")]
        public Channel[] channels { get; set; }
    }
}