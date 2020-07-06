using Decisions.Slack.Utility;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Decisions.Slack
{
    [DataContract]
    public class SlackChannel
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "is_channel")]
        public bool IsChannel { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "is_private")]
        public bool IsPrivate { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "is_general")]
        public bool IsGeneral { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "is_group")]
        public bool IsGroup { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "is_read_only")]
        public bool IsReadOnly { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "is_archived")]
        public bool IsArchived { get; set; }
    }

    internal class ChannelsListResponseModel : SlackResponseModel
    {
        public List<SlackChannel> Channels { get; set; }
    }

    internal class CreateChannelResponseModel : SlackResponseModel
    {
        public SlackChannel Channel { get; set; }
    }
}