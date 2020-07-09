using Decisions.Slack.Utility;
using DecisionsFramework.Design.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        [LongTextPropertyEditorAttribute]
        public string Text { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "ts")]
        public string Timestamp { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "pinned_to")]
        public string[] PinnedTo { get; set; }

        [DataMember]
        public bool IsPinned { get; set; }

        [DataMember]
        public DateTime DateTime
        {
            get
            {
                try
                {
                    var timetick = Convert.ToDouble(Timestamp, CultureInfo.InvariantCulture);
                    return DateTimeUtils.UnixTimeStampToDateTime(timetick);
                }
                catch
                {
                    return new DateTime(0);
                }
            }
        }
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