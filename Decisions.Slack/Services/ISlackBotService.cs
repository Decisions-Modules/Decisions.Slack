using System.ServiceModel;
using DecisionsFramework.ServiceLayer.Utilities;

namespace Decisions.Slack.Services;

[ServiceContract]
public interface ISlackBotService
{
    [OperationContract]
    void InitializeBot(AbstractUserContext userContext, SlackBot bot);

    [OperationContract]
    void DisconnectBot(AbstractUserContext userContext, SlackBot bot);

}