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
    [AutoRegisterStep("Search for Text in Channels", slackCategory)]
    [Writable]
    public class SearchForTextInChannels : AbstractStep
    {
        [PropertyHidden]
        public override DataDescription[] InputData
        {
            get
            {
                var data = new DataDescription[] { new DataDescription(typeof(string), textToSerachLabel), new DataDescription(typeof(int), matchCountLabel) };
                return base.InputData.Concat(data).ToArray();
            }
        }

        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                var data = new OutcomeScenarioData[] { new OutcomeScenarioData(resultOutcomeLabel, new DataDescription(typeof(SlackMatches[]), resultLabel)) };
                return base.OutcomeScenarios.Concat(data).ToArray();
            }
        }

        protected override Object ExecuteStep(string token, StepStartData data)
        {
            string text = (string)data.Data[textToSerachLabel];
            int maxMatchCount = (int)data.Data[matchCountLabel];

            return SlackClientApi.SearchForTextInChannels(token, text, maxMatchCount);
        }
    }
}
