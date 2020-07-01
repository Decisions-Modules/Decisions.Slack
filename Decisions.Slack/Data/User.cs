using System.Runtime.Serialization;

namespace SlackClient.Entities
{
    [DataContract]
    public class User
    {
        [DataMember(Name = "RealName")]
        public string real_name { get; set; }

        [DataMember(Name = "Id")]
        public string id { get; set; }
        
        [DataMember(Name = "Name")]
        public string name { get; set; }   
    }
}