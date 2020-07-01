using System.Runtime.Serialization;

namespace SlackClient.Entities
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