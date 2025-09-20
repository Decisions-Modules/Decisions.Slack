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
            log.Error($"Failed to create ApiClient for SlackBot Id: {bot.GetEntityId()}");
            return;
        }

        if (string.IsNullOrEmpty(bot.BotUserId))
        {
            log.Error($"Could not authenticate bot for SlackBot Id: {bot.GetEntityId()}");
            return;
        }

        _bot = bot;
        _api = bot.ApiClient;
        _botUserId = bot.BotUserId;
    }

    public async Task Handle(MessageEvent slackEvent)
    {
        // Only handle mentions
        if (string.IsNullOrEmpty(slackEvent.Text) || !slackEvent.Text.Contains($"<@{_botUserId}>"))
            return;
        
        // Include flow data
        List<KeyValuePair<string, object>> data = new List<KeyValuePair<string, object>>();
        data.Add(new KeyValuePair<string, object>(SlackBotConstants.MESSAGE_INPUT , slackEvent.Text));
        data.Add(new KeyValuePair<string, object>(SlackBotConstants.CHANNEL_INPUT, slackEvent.Channel));
        data.Add(new KeyValuePair<string, object>(SlackBotConstants.USER_INPUT, slackEvent.User));;
        
        // Run Flow
        Flow processingFlow = FlowEngine.LoadFlowByID(_bot.HandlerFlow, false, true);
        string flowTrackingId = FlowEngine.Start(processingFlow, new FlowStateData(data.ToArray()), false);
       
        // Pop instruction to be returned
        FlowExecutionStateInstruction instruction = FlowEngine.GetInstructionForCurrentUser(flowTrackingId);
        
        // Get the return value
        DataPair responseFromFlow = new();
        if (instruction is FlowCompletedInstruction)
        {
            FlowCompletedInstruction result = (FlowCompletedInstruction)instruction;
            
            if (result.ResultData != null && result.ResultData.Length > 0)
            {
                responseFromFlow = result.ResultData.FirstOrDefault(x => x.Name == SlackBotConstants.RESPONSE_PARAM) ?? new();
            }
        }
        
        // reply
        string? response = responseFromFlow.OutputValue.ToString();
        if (!string.IsNullOrEmpty(response))
        {
            await _api.Chat.PostMessage(new Message
            {
                Channel = slackEvent.Channel,
                Text = responseFromFlow.OutputValue.ToString()
            });
        }
    }
}