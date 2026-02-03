using Decisions.Slack.Data;
using Decisions.Slack.Utility;
using DecisionsFramework;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using DecisionsFramework.Design.Flow.Service.Execution;
using DecisionsFramework.ServiceLayer.Services.ContextData;
using SlackNet;
using SlackNet.Events;
using SlackNet.WebApi;

namespace Decisions.Slack.Bots.Handlers;

public class MentionHandler : IEventHandler<MessageEvent>
{
    private Log log = new Log(typeof(MentionHandler));
    private readonly SlackBot _bot;
    private readonly ISlackApiClient _api;
    private readonly string _botUserId;
    
    public MentionHandler(SlackBot bot)
    {
        if (bot.ApiClient == null)
        {
            throw new ArgumentNullException($"API Client cannot be null for SlackBot ID: {bot.GetEntityId()}" + 
                                            "Please validate the bot's configuration.");
        }

        if (string.IsNullOrEmpty(bot.BotUserId))
        {
            throw new ArgumentNullException($"User ID cannot be null for SlackBot ID: {bot.GetEntityId()}. " +
                                            "Please validate the bot's configuration.");
        }

        _bot = bot;
        _api = bot.ApiClient;
        _botUserId = bot.BotUserId;
    }

    public async Task Handle(MessageEvent slackEvent)
    {
        // Ignore messages from the bot itself to prevent infinite loops
        if (slackEvent.User == _botUserId)
            return;
        
        // Only handle mentions
        if (string.IsNullOrEmpty(slackEvent.Text) || !slackEvent.Text.Contains($"<@{_botUserId}>"))
            return;
        
        // Include flow data
        List<KeyValuePair<string, object>> data = new List<KeyValuePair<string, object>>();
        data.Add(new KeyValuePair<string, object>(SlackBotConstants.MESSAGE_INPUT , slackEvent.Text));
        data.Add(new KeyValuePair<string, object>(SlackBotConstants.CHANNEL_INPUT, slackEvent.Channel));
        data.Add(new KeyValuePair<string, object>(SlackBotConstants.USER_INPUT, slackEvent.User));
        data.Add(new KeyValuePair<string, object>(SlackBotConstants.THREAD_TIMESTAMP_INPUT, slackEvent.Ts ?? string.Empty));
        
        // Run Flow
        Flow processingFlow = FlowEngine.LoadFlowByID(_bot.HandlerFlow, false, true);
        FlowCompletedInstruction completedInstruction = FlowEngine.StartSyncFlow(processingFlow, new FlowStateData(data.ToArray()));
        
        // Get the return value
        DataPair responseFromFlow = new();
        if (completedInstruction != null)
        {
            if (completedInstruction.ResultData != null && completedInstruction.ResultData.Length > 0)
            {
                responseFromFlow = completedInstruction.ResultData.FirstOrDefault(
                    x => x.Name == SlackBotConstants.RESPONSE_PARAM) ?? new();
            }
        }
        
        // reply
        string? response = responseFromFlow.OutputValue.ToString();
        if (!string.IsNullOrEmpty(response))
        {
            await SendResponse(slackEvent, response);
        }
    }

    private async Task SendResponse(MessageEvent slackEvent, string response)
    {
        switch (_bot.ResponseType)
        {
            case SlackResponseType.Thread:
                await _api.Chat.PostMessage(new Message
                {
                    Channel = slackEvent.Channel,
                    Text = response,
                    ThreadTs = slackEvent.Ts // Reply in thread using the original message timestamp
                });
                break;
            default:
                // Default to channel response
                await _api.Chat.PostMessage(new Message
                {
                    Channel = slackEvent.Channel,
                    Text = response
                });
                break;
        }
    }
}