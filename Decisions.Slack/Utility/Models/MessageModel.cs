namespace SlackClient.Models
{
    internal class MessageModel
    {
        public string type { get; set; }
        public string user { get; set; }
        public string text { get; set; }
        public string ts { get; set; }
    }
}