using System.Runtime.Serialization;

namespace Decisions.Slack.Data
{
    [DataContract]
    public class HistoryRoot
    {
        [DataMember(Name = "Messages")]
        public Message[] messages { get; set; }
    }
}