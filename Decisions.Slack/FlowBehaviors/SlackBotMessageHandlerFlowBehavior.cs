using Decisions.Slack.Utility;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;

namespace Decisions.Slack.FlowBehaviors;

public class SlackBotMessageHandlerFlowBehavior : DefaultFlowBehavior, ISyncFlowBehavior
{
    public override string Name => "SlackBot Message Handler"; 
    public override bool ShowFlowInputs => true;
    public override bool ConstrainFlowOutputs => true;
    public override bool IsUserSettable => true;
    
    public override OutcomeScenarioData[] DefaultOutputs =>
    [
        new OutcomeScenarioData("Done", new DataDescription(new DecisionsNativeType(typeof(string)), 
            SlackBotConstants.RESPONSE_PARAM, false, false, false))
    ];

    public override DataDescription[] ProcessInputDeclaration(Flow flow, DataDescription[] inputData)
    {
        return
        [
            new DataDescription(typeof(string), SlackBotConstants.MESSAGE_INPUT),
            new DataDescription(typeof(string), SlackBotConstants.CHANNEL_INPUT),
            new DataDescription(typeof(string), SlackBotConstants.USER_INPUT),
        ];
    }
}