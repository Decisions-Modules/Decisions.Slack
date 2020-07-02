namespace Decisions.Slack.Models
{
    internal class MatchesModel
    {
        public ChannelModel channel { get; set; }
        public string type { get; set; }
        public string user { get; set; }
        public string text { get; set; }
        public string ts { get; set; }
        public string permalink { get; set; }
        public string team { get; set; }
        public string iid { get; set; }
    }
}