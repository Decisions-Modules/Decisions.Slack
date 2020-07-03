using Decisions.Slack.Utility;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Decisions.Slack.Data
{
    [DataContract]
    public class SlackUser
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "team_name")]
        public string TeamId { get; set; }

        [DataMember]
        public bool Deleted { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "real_name")]
        public string RealName { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "is_admin")]
        public bool IsAdmin { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "is_owner")]
        public bool IsOwner { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "is_primary_owner")]
        public bool IsPrimaryOwner { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "is_restricted")]
        public bool IsRestricted { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "is_ultra_restricted")]
        public bool IsUltraRestricted { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "is_bot")]
        public bool IsBot { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "is_stranger")]
        public bool IsStranger { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "is_app_user")]
        public bool IsAppUser { get; set; }

        [DataMember]
        public string Locale { get; set; }
    }

    internal class ChannelMembersResponseModel : SlackResponseModel
    {
        public string[] Members { get; set; }
    }

    internal class UserResponseModel : SlackResponseModel
    {
        public SlackUser User { get; set; }
    }
}