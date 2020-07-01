using System.Collections.Generic;

namespace SlackClient.Models
{
    internal class ChannelsListWithOkModel
    {
        public string ok { get; set; }
        public List<ChannelModel> channels { get; set; }
    }
}