using System.Runtime.Serialization;

namespace Decisions.Slack.Data
{
    [DataContract]
    public class ChannelsList
    {
        [DataMember(Name = "Channels")]
        public Channel[] channels { get; set; }
    }
}