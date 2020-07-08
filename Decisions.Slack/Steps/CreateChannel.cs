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
    [AutoRegisterStep("Create Channel", slackCategory)]
    [Writable]
    public class CreateChannel : AbstractStep
    {
        [PropertyHidden]
        public override DataDescription[] InputData
        {
            get
            {
                var data = new DataDescription[] { new DataDescription(typeof(string), channelLabel), new DataDescription(typeof(bool), isPrivateLabel), };
                return base.InputData.Concat(data).ToArray();
            }
        }

        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                var data = new OutcomeScenarioData[] { new OutcomeScenarioData(resultOutcomeLabel, new DataDescription(typeof(SlackChannel), resultLabel)) };
                return base.OutcomeScenarios.Concat(data).ToArray();
            }
        }

        protected override Object ExecuteStep(string token, StepStartData data)
        {
            string channelName = (string)data.Data[AbstractStep.channelLabel];
            bool isPrivate = (bool)data.Data[AbstractStep.isPrivateLabel];
            return SlackClientApi.CreateChannel(token, channelName, isPrivate);
        }
    }
}
