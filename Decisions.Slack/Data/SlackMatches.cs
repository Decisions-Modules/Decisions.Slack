using Decisions.Slack.Utility;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Decisions.Slack
{
    [DataContract]
    public class SlackMatches
    {
        [DataMember]
        public SlackChannel Channel { get; set; }

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
        public string Permalink { get; set; }

        [DataMember]
        public string Team { get; set; }

        [DataMember]
        public string Iid { get; set; }
    }

    internal class MatchesPagination
    {
        [JsonProperty(PropertyName = "first")]
        public int FirstItem { get; set; }

        [JsonProperty(PropertyName = "last")]
        public int LastItem { get; set; }

        [JsonProperty(PropertyName = "page")]
        public int CurrentPage { get; set; }

        [JsonProperty(PropertyName = "page_count")]
        public int PageCount { get; set; }

        [JsonProperty(PropertyName = "per_page")]
        public int PerPage { get; set; }

        [JsonProperty(PropertyName = "total_count")]
        public int TotalItemCount { get; set; }
    }

    internal class MatchesMessagesModel
    {
        public List<SlackMatches> Matches { get; set; }
        public MatchesPagination Pagination { get; set; }
    }

    internal class MatchesResponseModel : SlackResponseModel
    {
        public MatchesMessagesModel Messages { get; set; }
    }
}