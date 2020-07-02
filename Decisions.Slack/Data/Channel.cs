using System.Runtime.Serialization;

namespace Decisions.Slack.Data
{
    [DataContract]
    public class Channel
    {
        [DataMember(Name = "Id")]
        public string Id { get; set; }

        [DataMember(Name = "Name")]
        public string Name { get; set; }
    }
}