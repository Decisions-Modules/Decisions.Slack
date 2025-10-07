using Decisions.Slack.Bots.Handlers;
using Decisions.Slack.Utility;
using DecisionsFramework;
using DecisionsFramework.ServiceLayer.Services.PeerCache;
using DecisionsFramework.ServiceLayer.Utilities;
using DecisionsFramework.Utilities;
using SlackNet;
using SlackNet.WebApi;

namespace Decisions.Slack.Services;

[AutoRegisterService("SlackBotService", typeof(ISlackBotService))]
public class SlackBotService : ISlackBotService
{
    private readonly Log _log = new Log(typeof(SlackBotService));
    private static ClusterAwareDictionary<SlackBot> _bots = new ClusterAwareDictionary<SlackBot>(SlackBotConstants.SLACK_BOT_CACHE_NAME);

    public static ISlackBotService Instance
    {
        get
        {
            return ServicesHolder.GetService<ISlackBotService>(() => new SlackBotService());
        }
    }
    
    public void InitializeBot(AbstractUserContext userContext, SlackBot bot)
    {
        _bots.TryGetValue(bot.GetEntityId(), out SlackBot? foundBot);
        if (foundBot != null)
        {
            if (foundBot.Client?.Connected == true)
            {
                _log.Warn($"Bot {bot.GetEntityId()} is already connected. Reconnecting.");
                DisconnectBot(userContext, foundBot);
            }
        }
        
        // Initialize Slack API client.
        SlackServiceBuilder app = new SlackServiceBuilder()
            .UseApiToken(bot.BotToken)
            .UseAppLevelToken(bot.AppToken);

        _bots[bot.GetEntityId()] = bot;
        
        bot.ApiClient = app.GetApiClient();

        // Test authentication to get current user id
        AuthTestResponse me = bot.ApiClient.Auth.Test().GetAwaiter().GetResult();
        bot.BotUserId = me.UserId;
        
        // Register event handler.
        // MentionHandler will handle all messages that mention the bot.
        app.RegisterEventHandler(new MentionHandler(bot));

        bot.Client = app.GetSocketModeClient();
        bot.Client.Connect();
    }

    public void DisconnectBot(AbstractUserContext userContext, SlackBot bot)
    {
        _bots.TryGetValue(bot.GetEntityId(), out var foundBot);
        foundBot?.Client?.Disconnect();
        bool removed = _bots.Remove(bot.GetEntityId(), out SlackBot? removedBot);

        if (removed && removedBot != null)
        {
            _log.Info($"Bot {removedBot?.GetEntityId()} is disconnected.");
        }
    }
}