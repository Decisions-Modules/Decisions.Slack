using Decisions.Slack.Utility;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Decisions.Slack
{
    [DataContract]
    public class SlackMessage
    {
        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public string User { get; set; }

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "ts")]
        public string Timestamp { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "pinned_to")]
        public string[] PinnedTo { get; set; }
    }

    internal class MessageListResponseModel : SlackResponseModel
    {
        public List<SlackMessage> Messages { get; set; }
    }

    internal class PostMessageResponseModel : SlackResponseModel
    {
        public SlackMessage Message { get; set; }
    }
}