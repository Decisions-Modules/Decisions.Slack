using System.Runtime.Serialization;

namespace Decisions.Slack.Data
{
    [DataContract]
    public class RootUserInfo
    {
        [DataMember(Name = "User")]
        public User user { get; set; }
    }
}