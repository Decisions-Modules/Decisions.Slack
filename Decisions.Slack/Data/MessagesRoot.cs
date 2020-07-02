using System.Runtime.Serialization;

namespace Decisions.Slack.Data
{
    [DataContract]
    public class MessagesRoot
    {
        [DataMember(Name = "Querey")]
        public string query { get; set; }

        [DataMember(Name = "Messages")]
        public MessagesSearch messages { get; set; }
    }
}