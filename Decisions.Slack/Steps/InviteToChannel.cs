﻿using Decisions.Slack.Utility;
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
    [AutoRegisterStep("Invite to Channel", slackCategory)]
    [Writable]
    public class InviteToChannel : AbstractStep
    {
        [PropertyHidden]
        public override DataDescription[] InputData
        {
            get
            {
                var data = new DataDescription[] { new DataDescription(typeof(string), channelIdLabel), new DataDescription(typeof(string[]), userIdsLabel), };
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
            string channelId = (string)data.Data[AbstractStep.channelIdLabel];
            string[] userIds = (string[])data.Data[AbstractStep.userIdsLabel];
            return SlackClientApi.InviteToChannel(token, channelId, userIds);
        }
    }
}
