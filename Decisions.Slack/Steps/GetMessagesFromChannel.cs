using Decisions.Slack.Utility;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using DecisionsFramework.Design.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decisions.Slack
{
    [AutoRegisterStep("Get Messages from Channel", SlackCategory)]
    [Writable]
    public class GetMessagesFromChannel : AbstractStep
    {
        [PropertyHidden]
        public override DataDescription[] InputData
        {
            get
            {
                var data = new DataDescription[] { new DataDescription(typeof(string), CHANNEL_ID),
                    new DataDescription(typeof(int), MESSAGE_COUNT),
                    new DataDescription(typeof(string), LATEST_MESSAGE_TIMESTAMP), };
                return base.InputData.Concat(data).ToArray();
            }
        }

        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                var data = new OutcomeScenarioData[] { new OutcomeScenarioData(RESULT_OUTCOME, new DataDescription(typeof(SlackMessage[]), RESULT)) };
                return base.OutcomeScenarios.Concat(data).ToArray();
            }
        }

        protected override Object ExecuteStep(string token, StepStartData data)
        {
            string channelId = (string)data.Data[CHANNEL_ID];
            int maxMessageCount = (int)data.Data[MESSAGE_COUNT];
            string timestamp = (string)data.Data[LATEST_MESSAGE_TIMESTAMP];
            return SlackClientApi.GetMessagesFromChannel(token, channelId, maxMessageCount, timestamp);
        }
    }
}
