using System.Runtime.Serialization;

namespace Decisions.Slack.Data
{
    [DataContract]
    public class MessagesSearch
    {
        [DataMember(Name = "Matches")]
        public Matches[] matches { get; set; }

        [DataMember(Name = "Total")]
        public int total { get; set; }
    }
}