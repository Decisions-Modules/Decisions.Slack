using System.Runtime.Serialization;

namespace SlackClient.Entities
{
    [DataContract]
    public class Channel
    {
        [DataMember(Name = "Id")]
        public string id { get; set; }

        [DataMember(Name = "Name")]
        public string name { get; set; }
    }
}