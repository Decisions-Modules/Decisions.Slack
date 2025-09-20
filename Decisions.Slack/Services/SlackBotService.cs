using Decisions.Slack.Bots.Handlers;
using DecisionsFramework;
using DecisionsFramework.ServiceLayer.Utilities;
using DecisionsFramework.Utilities;
using SlackNet;

namespace Decisions.Slack.Services;

[AutoRegisterService("SlackBotService", typeof(ISlackBotService))]
public class SlackBotService : ISlackBotService
{
    private Log log = new Log(typeof(SlackBotService));
    private Dictionary<string, SlackBot> _bots = new Dictionary<string, SlackBot>();

    public static ISlackBotService Instance
    {
        get { return ServicesHolder.GetService<ISlackBotService>(() => new SlackBotService()); }
    }
    
    public void InitializeBot(AbstractUserContext userContext, SlackBot bot)
    {
        _bots.TryGetValue(bot.GetEntityId(), out var foundBot);
        if (foundBot != null)
        {
            if (foundBot.Client?.Connected == true)
            {
                log.Warn($"Bot {bot.GetEntityId()} is already connected. Reconnecting.");
                DisconnectBot(userContext, foundBot);
            }
        }
        
        // Initialize Slack API client.
        var app = new SlackServiceBuilder()
            .UseApiToken(bot.BotToken)
            .UseAppLevelToken(bot.AppToken);

        _bots[bot.GetEntityId()] = bot;
        
        bot.ApiClient = app.GetApiClient();

        // Test authentication to get current user id
        var me = bot.ApiClient.Auth.Test().GetAwaiter().GetResult();
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
        _bots.Remove(bot.GetEntityId());
    }
}