using System.Runtime.Serialization;

namespace SlackClient.Entities
{
    [DataContract]
    public class RootUserInfo
    {
        [DataMember(Name = "User")]
        public User user { get; set; }
    }
}