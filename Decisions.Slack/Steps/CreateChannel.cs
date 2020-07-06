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
    [AutoRegisterStep("Create Channel", SlackCategory)]
    [Writable]
    public class CreateChannel : AbstractStep
    {
        [PropertyHidden]
        public override DataDescription[] InputData
        {
            get
            {
                var data = new DataDescription[] { new DataDescription(typeof(string), CHANNEL_NAME), new DataDescription(typeof(bool), IS_PRIVATE), };
                return base.InputData.Concat(data).ToArray();
            }
        }

        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                var data = new OutcomeScenarioData[] { new OutcomeScenarioData(RESULT_OUTCOME, new DataDescription(typeof(SlackChannel), RESULT)) };
                return base.OutcomeScenarios.Concat(data).ToArray();
            }
        }

        protected override Object ExecuteStep(string token, StepStartData data)
        {
            string channelName = (string)data.Data[CHANNEL_NAME];
            bool isPrivate = (bool)data.Data[IS_PRIVATE];
            return SlackClientApi.CreateChannel(token, channelName, isPrivate);
        }
    }
}
