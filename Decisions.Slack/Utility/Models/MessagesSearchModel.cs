namespace SlackClient.Models
{
    internal class MessagesSearchModel
    {
        public MatchesModel[] matches { get; set; }
        public int total { get; set; }
    }
}