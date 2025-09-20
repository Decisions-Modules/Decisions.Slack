namespace Decisions.Slack.Utility;

internal static class SlackEndpointNames
{
    public const string postMessage = "chat.postMessage";
    public const string createChannel = "conversations.create";
    public const string inviteToChannel = "conversations.invite";
    public const string conversationsList = "conversations.list";
    public const string conversationsMembers = "conversations.members";
    public const string conversationsHistory = "conversations.history";
    public const string conversationsOpen = "conversations.open"; // post direct message to user-s
    public const string archiveChannel = "conversations.archive";
    public const string searchInChannels = "search.messages";
    public const string deleteMsgFromChannel = "chat.delete";
    public const string usersInfo = "users.info";
    public const string pinMsgToChannel = "pins.add";
    public const string removePinMsgToChannel = "pins.remove";
}