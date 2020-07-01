namespace SlackClient.Models
{
    internal class MessagesRootModel
    {
        public bool ok { get; set; }
        public string query { get; set; }

        public MessagesSearchModel messages { get; set; }
    }
}