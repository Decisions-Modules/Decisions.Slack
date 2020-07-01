using System.Runtime.Serialization;

namespace SlackClient.Entities
{
    [DataContract]
    public class MembersRoot
    {
        [DataMember(Name = "Members")]
        public string[] members { get; set; }
    }
}