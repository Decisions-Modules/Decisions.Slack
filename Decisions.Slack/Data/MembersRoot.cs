using System.Runtime.Serialization;

namespace Decisions.Slack.Data
{
    [DataContract]
    public class MembersRoot
    {
        [DataMember(Name = "Members")]
        public string[] members { get; set; }
    }
}