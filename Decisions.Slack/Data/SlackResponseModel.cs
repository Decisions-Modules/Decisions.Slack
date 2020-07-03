using Newtonsoft.Json;
using System;
using System.Net;

namespace Decisions.Slack.Data
{
    internal class SlackMetadataModel
    {
        [JsonProperty(PropertyName = "next_cursor")]
        public string NextCursor { get; set; }
    }

    internal class SlackResponseModel
    {
        public bool Ok { get; set; }
        public string Error { get; set; }

        [JsonProperty(PropertyName = "response_metadata")]
        public SlackMetadataModel Metadata { get; set; }
    }


}